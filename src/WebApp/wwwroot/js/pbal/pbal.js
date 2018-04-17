var CC = CC || {};
CC.CORE = CC.CORE || {};

CC.CORE.Cache = (function () {
    var defaultCacheExpiry = 15 * 60 * 1000; // default is 15 minutes
    var aMinuteInMs = (1000 * 60);
    var anHourInMs = aMinuteInMs * 60;

    var getCacheObject = function () {
        // Using session storage rather than local storage as caching benefit
        // is minimal so would rather have an easy way to reset it.
        return window.sessionStorage;
    };

    var isSupportStorage = function () {
        var cacheObj = getCacheObject();
        var supportsStorage = cacheObj && JSON && typeof JSON.parse === "function" && typeof JSON.stringify === "function";
        if (supportsStorage) {
            // Check for dodgy behaviour from iOS Safari in private browsing mode
            try {
                var testKey = "candc-cache-isSupportStorage-testKey";
                cacheObj[testKey] = "1";
                cacheObj.removeItem(testKey);
                return true;
            }
            catch (ex) {
                // Private browsing mode in iOS Safari, or possible full cache
            }
        }
        CC.CORE.Log("PBAL: Browser does not support caching");
        return false;
    };

    var getExpiryKey = function (key) {
        return key + "_expiry";
    };

    var isCacheExpired = function (key) {
        var cacheExpiryString = getCacheObject()[getExpiryKey(key)];
        if (typeof cacheExpiryString === "string" && cacheExpiryString.length > 0) {
            var cacheExpiryInt = parseInt(cacheExpiryString);
            if (cacheExpiryInt > (new Date()).getTime()) {
                return false;
            }
        }
        return true;
    };

    var get = function (key) {
        if (isSupportStorage()) {
            if (!isCacheExpired(key)) {
                var valueString = getCacheObject()[key];
                if (typeof valueString === "string") {
                    CC.CORE.Log("PBAL: Got from cache at key: " + key);
                    if (valueString.indexOf("{") === 0 || valueString.indexOf("[") === 0) {
                        var valueObj = JSON.parse(valueString);
                        return valueObj;
                    }
                    else {
                        return valueString;
                    }
                }
            }
            else {
                // remove expired entries?
                // not required as we will almost always be refreshing the cache
                // at this time
            }
        }
        return null;
    };

    var set = function (key, valueObj, validityPeriodMs) {
        var didSetInCache = false;
        if (isSupportStorage()) {
            // Get value as a string
            var cacheValue = undefined;
            if (valueObj === null || valueObj === undefined) {
                cacheValue = null;
            }
            else if (typeof valueObj === "object") {
                cacheValue = JSON.stringify(valueObj);
            }
            else if (typeof valueObj.toString === "function") {
                cacheValue = valueObj.toString();
            }
            else {
                alert("PBAL: Cannot cache type: " + typeof valueObj);
            }

            // Cache value if it is valid
            if (cacheValue !== undefined) {
                // Cache value
                getCacheObject()[key] = cacheValue;
                // Ensure valid expiry period
                if (typeof validityPeriodMs !== "number" || validityPeriodMs < 1) {
                    validityPeriodMs = defaultCacheExpiry;
                }
                // Cache expiry
                getCacheObject()[getExpiryKey(key)] = ((new Date()).getTime() + validityPeriodMs).toString();
                CC.CORE.Log("PBAL: Set in cache at key: " + key);
                didSetInCache = true;
            }
        }
        return didSetInCache;
    };

    var clear = function (key) {
        var cache = getCacheObject();
        if (key) {
            cache.removeItem(key);
            cache.removeItem(getExpiryKey(key));
        } else {
            var keys = [];
            for (var i = 0; i < cache.length; i++) {
                if (cache.key(i).indexOf("candc_cache_PBAL") >= 0) {
                    keys.push(cache.key(i));
                }
            }
            for (var i = 0; i < keys.length; i++) {
                cache.removeItem(keys[i]);
            }
        }
    };

    return {
        Get: get,
        Set: set,
        Clear: clear,
        IsSupportStorage: isSupportStorage,
        Timeout: {
            VeryShort: (aMinuteInMs * 1),
            Default: (anHourInMs * 2),
            VeryLong: (anHourInMs * 72),
        }
    };
})();

CC.CORE.Log = function (errMsg) {
    // console.log is undefined in IE10 and earlier unless in debug mode, so must check for it
    if (typeof window.console === "object" && typeof console.log === "function") {
        console.log(errMsg);
    }
};

CC.CORE.PBAL = (function () {
    "use strict";
    
    var appTokenFactory = function (aadAppClientId, tenant, b2cScope, b2cPolicy, userId, redirectUrl) {
        // NOTE on security: include the userId in the cache key to prevent the case where a user logs out but
        // leaves the tab open and a new user logs in on the same tab. The first user's calender
        // would be returned if we didn't associate the cache key with the current user.
        var cacheKey = "candc_cache_PBAL_" + userId + "_" + aadAppClientId;

        this.params = {
            clientId: aadAppClientId,
            tenant: tenant,
            redirectUrl: redirectUrl,
            cacheKey: cacheKey,
            scope: b2cScope,
            policy: b2cPolicy,
            oid: userId
        };

        var getAuthorizeUri = function (params, redirectUrl) {
            var b2cAuthUri = "https://login.microsoftonline.com/te/" + params.tenant + "/" + params.policy + "/oauth2/v2.0/authorize?" +
                "client_id=" + params.clientId +
                "&response_type=token" +
                "&redirect_uri=" + encodeURIComponent(redirectUrl) +
                "&scope=" + encodeURIComponent(params.scope) +
                "&response_mode=fragment" +
                "&state=12345" +
                "&nonce=12345" +
                "&prompt=none" +
                "&domain_hint=organizations" +
                "&login_hint=" + params.oid;
            return b2cAuthUri;
        };

        var getQueryStringParameterByName = function (name, url) {
            name = name.replace(/[\[\]]/g, "\\$&");
            var regex = new RegExp("[?&#]" + name + "(=([^&#]*)|&|#|$)");
            var results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, " "));
        };

        // create iframe, set its href, set listener for when loaded
        // to parse the query string. Deferred returns upon parse of query string in iframe.
        var acquirePassiveToken = function (params) {
            var deferred = jQuery.Deferred();

            // create iframe and inject into dom
            var iframe = jQuery("<iframe />").attr({
                width: 1,
                height: 1,
                src: getAuthorizeUri(params, params.redirectUrl),
                style: 'visibility: hidden;'
            })
            jQuery(document.body).append(iframe);

            // bind event handler to iframe for parse query string on load
            iframe.on("load", function (iframeData) {
                parseAccessTokenFromIframe(iframeData, deferred);
            });

            return deferred.promise();
        };

        // handle iframe once it has loaded
        var parseAccessTokenFromIframe = function (iframeData, deferred) {
            // read the iframe href
            var frameHref = "";
            try {
                // this will throw a cross-domain error for any issue other than success
                // as the iframe will diplay the error on the login.microsoft domain
                frameHref = iframeData.currentTarget.contentWindow.location.href;
            }
            catch (error) {
                //deferred.reject(error);
                //return;
            }

            // parse iframe query string parameters
            var error = getQueryStringParameterByName("error", frameHref);
            var errorDescription = getQueryStringParameterByName("error_description", frameHref);
            var accessToken = getQueryStringParameterByName("access_token", frameHref);
            var expiresInSeconds = getQueryStringParameterByName("expires_in", frameHref);
            var state = getQueryStringParameterByName("state", frameHref);
            var type = getQueryStringParameterByName("token_type", frameHref);

            var response = {
                accessToken: accessToken,
                expiresInSeconds: expiresInSeconds,
                state: state,
                tokenType: type,
                error: error,
                errorDescription: errorDescription
            };

            // delete the iframe, and event handler.
            var iframe = jQuery(iframeData.currentTarget);
            iframe.remove();

            // check for error in iframe response
            if (response.error !== null) {
                // throw error with response data returned
                deferred.reject(response);
            }

            // resolve promise
            deferred.resolve(response);
        };

        this.GetHeaders = function (token, additionalHeaders) {
            var ajaxHeaders = {};

            // check token for error
            if (token.error !== null) {
                CC.CORE.Log("PBAL: token error detected: " + token.error + " | not including authoriztion header");
            } else if (token.tokenType !== null && token.accessToken !== null) {
                CC.CORE.Log("PBAL: token detected: " + token.tokenType + " | adding authoriztion header");

                ajaxHeaders = {
                    'Authorization': token.tokenType + ' ' + token.accessToken
                };
            } else {
                CC.CORE.Log("PBAL: token object attributes missing | not including authoriztion header");
            }

            // add additional headers if they exist
            if (typeof additionalHeaders === "object") {
                jQuery.extend(ajaxHeaders, additionalHeaders);
            }

            return ajaxHeaders;
        }

        // get the most recent token from the cache, or if not available,
        // fetch a new token via iframe
        this.GetToken = function () {
            var deferred = jQuery.Deferred();

            var params = this.params;

            if (params.oid == "" || params.oid == null) {
                // clear cache to avoid stale access tokens being available
                CC.CORE.Log("PBAL: clear cache");
                CC.CORE.Cache.Clear();
                deferred.reject({
                    error: "no user detected",
                    errorDescription: "user id in Portal hidden input was null or not found"
                });
                return deferred.promise();
            }

            // check for cached token
            var tokenFromCache = CC.CORE.Cache.Get(params.cacheKey);
            if (!tokenFromCache) {
                // fetch token via iframe
                acquirePassiveToken(params)
                    .done(function (tokenFromIframe) {
                        CC.CORE.Log("PBAL: Fetched token from iframe.");
                        // expire cache a minute before token expires to be safe
                        var cacheTimeout = (tokenFromIframe.expiresInSeconds - 60) * 1000;
                        CC.CORE.Cache.Set(params.cacheKey, tokenFromIframe, cacheTimeout);
                        // resolve the promise
                        deferred.resolve(tokenFromIframe);
                    })
                    .fail(function (errorResponse) {
                        // Logs when rejection is caught
                        deferred.reject(errorResponse);
                    });
            }
            else {
                CC.CORE.Log("PBAL: Fetched token from cache.");
                // resolve the promise
                deferred.resolve(tokenFromCache);
            }
            return deferred.promise();
        };
    };

    CC.CORE.Log("token factory loaded");

    return {
        AppTokenFactory: appTokenFactory
    };

})(jQuery);