# Dynamics Portal Companion App 
#### *BETA RELEASE*

Dynamics Portal Companion App is a starter template for a companion web app for Dynamics 365 portals.  This template includes an authenticated experience persisting the portal user across requests between the Dynamics 365 portal and the Dynamics Portal Companion App web app using Azure AD B2C.

Dynamics Portal Companion App is built on ASP.NET Core 1.1 with the .NET Framework 4.6.1.  It includes the Dynamics 365 SDK and the Dynamics 365 Xrm Tooling utlizing the CrmServiceClient implemented for ASP.NET Core as CrmCoreServiceClient.

 **Beta Release Notes**
* The version of PBAL currently does not do JWT validation.  This includes excluding `nonce` and `state`.  JWT is validated on the Dynamics Portal Companion App with ASP.NET middleware.  Token should not be used outside of validation with the configured ASP.NET middleware.  Intent is to replace PBAL with Microsoft Authentication Library (MSAL) once it exits preview and better supports the Dynamics 365 portal scenario.

## Objective

Dynamics Portal Companion App provides a way to extend the Dynamics 365 portal with custom code in a secure manner.  This is accomplished by abstracting the Dynamics 365 portal authentication to a Single Sign On or Secure Token Service so that the user identity can be shared across applications.  This template implements the now natively Dynamics 365 portal supported Azure AD B2C.  Azure AD B2C has also been announced to replace all authentication for the Dynamics 365 portal in the near future.

Utilizing JavaScript front-end frameworks like [angular.js](https://angular.io/), [react.js](https://facebook.github.io/react/), and [vue.js](https://vuejs.org/) you can create seamless user experiences with this template that utilize custom server side code with the .NET Framework and Dynamics 365 SDK.

## Components

#### src/WebApp
Starter Template based on ASP.NET Core 1.1 with .NET Framework 4.6.1 Web Application

#### solutions
Starter solutions by Dynamics Portal Companion App Team:
* Dynamics Portal Companion App Base - includes default security role for use with S2S user or Connection String user to allow sample code to execute.

#### liquid
Liquid Templates to be added to Dynamics 365 portals configuration for easy component installation
* dynpca-core.liquid - core template for the portal companion loader.  Loads portal user identity, website, and companion app URI and loads base JavaScript module from companion app.  
	* Create new web template
	* Appended to website footer web template using liquid include tag. `{% include 'DynPCA - Core' %}`
* dynpca-apisdktest.liquid - example implementation of an authenticated and unauthenticated API call using `ExecuteRequest`

## Data

#### Site Settings
The following Dynamics 365 portal site settings are required to be added and configured with your Dynamics Portal Companion App information:
* `DynPCA/Auth/Authority` - the Azure AD B2C authority.  Same value as `Authentication/OpenIdConnect/AzureADB2C/Authority`
* `DynPCA/Uri` - the fully qualified URI to your Dynamics Portal Companion App eg. `https://pca.azurewebsites.net`

Session Management for Azure AD B2C:
* `Authentication/ApplicationCookie/ExpireTimeSpan` - should match your B2C session management policy configuration as a timespan.  Within an B2C policy edit, select Token, session, & SSO config, Access & ID token lifetime (minutes) - "The lifetime of the OAuth bear token used to gain access to the protected resource"
* `Authentication/ApplicationCookie/SlidingExpiration` - set to `false`


## Building

To build the project, ensure that you have [Git](https://git-scm.com/downloads) installed to obtain the source code, and [Visual Studio 2017](https://docs.microsoft.com/en-us/visualstudio/welcome-to-visual-studio) with [ASP.NET Core 2.0](https://www.microsoft.com/net/download/core) installed to compile the source code.

- Clone the repository using Git:
  ```sh
  git clone https://github.com/Adoxio/dynamics-portal-companion-app.git
  ```
- Open the `PortalBuddyWebApp.sln` solution file in Visual Studio

## License

This project uses the [MIT license](https://opensource.org/licenses/MIT).

## Contributions

This project accepts community contributions through GitHub, following the [inbound=outbound](https://opensource.guide/legal/#does-my-project-need-an-additional-contributor-agreement) model as described in the [GitHub Terms of Service](https://help.github.com/articles/github-terms-of-service/#6-contributions-under-repository-license):
> Whenever you make a contribution to a repository containing notice of a license, you license your contribution under the same terms, and you agree that you have the right to license your contribution under those terms.

Please submit one pull request per issue so that we can easily identify and review the changes.

## Support
This project has only been tested with Dynamics 365 versions 8.2 and 9.0 with portal version 8.3.