using System.Collections.Generic;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Okta.AspNet;
using Owin;


[assembly: OwinStartup(typeof(okta_aspnet_mvc_example.Startup))]

namespace okta_aspnet_mvc_example
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            // Add Refresh Middleware
            app.Use(typeof(RefreshTokenMiddleware));

            app.UseOktaMvc(new OktaMvcOptions()
            {
                OktaDomain = "https://{domain}.okta.com",
                ClientId = "0oa...",
                ClientSecret = "Klf...",
                AuthorizationServerId = "default",
                RedirectUri = "https://localhost:44314/interactioncode/callback",
                PostLogoutRedirectUri = "https://localhost:44314",
                GetClaimsFromUserInfoEndpoint = true,
                Scope = new List<string> {"openid", "profile", "email", "offline_access"}
            });
        }
    }
}
