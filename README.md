SAML2-ASP.NET
==============

Recently I've been working in a ASP.NET MVC project that delegates the users authentication to OKTA. To do that, I've used the library [SAML2](https://saml2.codeplex.com/) for ASP.NET.

The SAML2 Library works perfectly. You don't need to do extra code to make it work since it works as a middleware, but you need to configure it properly to work with your application. As there are very few examples in the documentation of how to configure it, I've decided to share the configuration of my ASP.NET MVC 4.6 application configured with SAML2.

---

## Installation

To install the SAML2 library in your project, you may either download the source package and compile it, or install from NuGet package "SAML2".
I've installed it from the NuGet package.

## Configuration Web.config

The SAML2 library will require several Web.config changes. One new section must be added to the Web.config, and three handlers need to be mapped to use this library.

In the system.web section we need to configure the [authentication](https://msdn.microsoft.com/en-us//library/aa291347(v=vs.71).aspx) mode to forms: 
"The Forms authentication provider is an authentication scheme that makes it possible for the application to collect credentials using an HTML form directly from the client. The client submits credentials directly to your application code for authentication. If your application authenticates the client, it issues a cookie to the client that the client presents on subsequent requests. If a request for a protected resource does not contain the cookie, the application redirects the client to the logon page"

Finally we need to add the Saml20MetadataFetcher Module. This module comes with the SAML2 library. Without it, I had a problem getting the OKTA response correctly because I think this module is the one that is in charge of parse the OKTA response.

```csharp

<configuration>
  <configSections>
    <section name="saml2" type="SAML2.Config.Saml2Section, SAML2" />
  </configSections>
  
  <system.web>
    <authentication mode="Forms" />
  </system.web>
  
  <system.webServer>
    <handlers>
      <remove name="SAML2.Protocol.Saml20SignonHandler" />
      <remove name="SAML2.Protocol.Saml20LogoutHandler" />
      <remove name="SAML2.Protocol.Saml20MetadataHandler" />
      <add name="SAML2.Protocol.Saml20SignonHandler" verb="*" path="Login.ashx" type="SAML2.Protocol.Saml20SignonHandler, SAML2" />
      <add name="SAML2.Protocol.Saml20LogoutHandler" verb="*" path="Logout.ashx" type="SAML2.Protocol.Saml20LogoutHandler, SAML2" />
      <add name="SAML2.Protocol.Saml20MetadataHandler" verb="*" path="Metadata.ashx" type="SAML2.Protocol.Saml20MetadataHandler, SAML2" />
    </handlers>
    <modules>
      <remove name="Saml20MetadataFetcher" />
      <add name="Saml20MetadataFetcher" type="SAML2.Saml20MetadataFetcherModule" preCondition="managedHandler" />
    </modules>
  </system.webServer>
  
  <saml2>
  ...
  </saml>

```



