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

## Section SAML2

The saml2 configuration section is split into several specific configuration areas. You have to configure at least 3 of them, the rest are optionals. With those 3 I had my SAML2 section working.

#### allowedAudienceUris [+Info](https://saml2.codeplex.com/wikipage?title=AllowedAudienceUris%20Element&referringTitle=Documentation)
"Configures the SAML AudienceRestrictions values to use when generating assertions."

We need to configure here the ID of the service provider.

#### serviceProvider [+Info](https://saml2.codeplex.com/wikipage?title=ServiceProvider%20Element&referringTitle=Documentation)
"Configures the service provider information such as endpoints, SAML AuthnContexts, and NameIdFormats."

Again we need to configure here the ID of the service provider.

In my case (OKTA) I was required to get a auto signed certificate, if that's your case, you can use the default certificate of your machine if you are in Windows.

In the endpoints section we need to set up the paths of the ashx files and set the redirectUrl when the action succeded. In this example I set up a redirectUrl to my home controller and this is the one which allows or not the access. You can see more of that controller in the MVC Controller section.

#### identityProviders [+Info](https://saml2.codeplex.com/wikipage?title=IdentityProviders%20Element&referringTitle=Documentation)
"Configures the identity providers to be used for federation. Can be as simple as defining the location of the IdP metadata, or provide access to overriding IdP metadata with custom values, etc."

Finally we need to specify the configuration information of the Identity Providers. In my case I override the default Provider settings by adding them manually

The parameter "IDPS_DIRECTORY" is the Directory in which we have the metadata of the federation partners.
And the parameter "OKTA_ENDPOINT" is the url of okta where you can set the idp.

```csharp
<saml2>
    <allowedAudienceUris>
      <audience uri="http://localhost:8888/" />
    </allowedAudienceUris>

    <serviceProvider id="http://localhost:8888/" server="http://localhost:8888/">
       <signingCertificate findValue="CN=localhost" storeLocation="LocalMachine" storeName="My" x509FindType="FindBySubjectDistinguishedName" />
      <endpoints>
        <endpoint type="SignOn" localPath="Login.ashx" redirectUrl="~/home/private" />
        <endpoint type="Logout" localPath="Logout.ashx" redirectUrl="~/home/index" />
        <endpoint type="Metadata" localPath="Metadata.ashx" />
      </endpoints>

      <authenticationContexts comparison="Exact">
        <add context="urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport" referenceType="AuthnContextClassRef" />
      </authenticationContexts>
    </serviceProvider>
    
    <identityProviders metadata="IDPS_DIRECTORY">
      <add id="OKTA_ENDPOINT" default="true">
        <certificateValidations>
          <add type="SAML2.Specification.SelfIssuedCertificateSpecification, SAML2" />
        </certificateValidations>
      </add>
    </identityProviders>
  </saml2>
```

## MVC RouteConfig

## MVC Controllers
