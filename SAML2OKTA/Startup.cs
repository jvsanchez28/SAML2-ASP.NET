using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SAML2OKTA.Startup))]
namespace SAML2OKTA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            ConfigureAuth(app);
        }
    }
}
