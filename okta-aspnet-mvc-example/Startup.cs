using System.Collections.Generic;
using System.Configuration;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Okta.AspNet;
using Owin;

//using Microsoft.AspNet.Identity;

[assembly: OwinStartup(typeof(okta_aspnet_mvc_example.Startup))]

namespace okta_aspnet_mvc_example
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            /*app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });*/

            app.Use(typeof(RefreshTokenMiddleware));

            app.UseOktaMvc(new OktaMvcOptions()
            {
                OktaDomain = "https://oie.erikdevelopernot.com",
                ClientId = "0oa2cpl777xczKzL21d7",
                ClientSecret = "Klf03TzBqEuayATkPGy7VgTqyNDKIPsIYNd9TEKo",
                AuthorizationServerId = "default",
                RedirectUri = "https://localhost:44314/interactioncode/callback",
                PostLogoutRedirectUri = "https://localhost:44314",
                GetClaimsFromUserInfoEndpoint = true,
                Scope = new List<string> {"openid", "profile", "email", "offline_access"},
            });
        }
    }
}
