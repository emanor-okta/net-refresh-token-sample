# ASP.NET MVC & Okta-Hosted Login Page Example

This example shows you how to use the `Okta.AspNet` library to log in a user. The user's browser is first redirected to the Okta-hosted login page. After the user authenticates, they are redirected back to your application. ASP.NET MVC automatically populates `HttpContext.User` with the information Okta sends back about the user.

## Token Refresh Logic Added - (Extend user ASP.NET session)

This builds on the Okta sample by adding token refresh abilities using the solution provided at [refresh_token.md](https://github.com/okta/okta-aspnet/blob/master/docs/refresh-token.md).
Notable changes:
* [RefreshTokenMiddleware.cs](https://github.com/emanor-okta/net-refresh-token-sample/blob/main/okta-aspnet-mvc-example/RefreshTokenMiddleware.cs)
* Adding middleware in [Startup.cs](https://github.com/emanor-okta/net-refresh-token-sample/blob/main/okta-aspnet-mvc-example/Startup.cs#L20)

By default Okta mints id_tokens with a 60 minute expiry time. This is the value ASP.NET uses to create a users session. In order for ASP.NET to refresh a users tokens, a users needs to be actively accessing the application and a decision of when to refresh tokens needs to be implemented. In this sample a refresh is done in the [last 15 minutes of session lifetime](https://github.com/emanor-okta/net-refresh-token-sample/blob/main/okta-aspnet-mvc-example/RefreshTokenMiddleware.cs#L79). This can be modified as needed.
There is also [non-production ready sample](https://github.com/emanor-okta/net-refresh-token-sample/blob/main/okta-aspnet-mvc-example/Views/Shared/_Layout.cshtml#L63) to show how JS could actively send a heartbeat to the application to keep a session active if it is desired to allow long idle times by users.

## Prerequisites

Before running this sample, you will need the following:

* An Okta Developer Account, you can sign up for one at https://developer.okta.com/signup/.
* An Okta Application, configured for Web mode. This is done from the Okta Developer Console and you can find instructions [here][OIDC Web Application Setup Instructions].  When following the wizard, use the default properties.  They are designed to work with our sample applications.


## Running This Example

Clone this repo and replace the okta configuration placeholders in the `Web.Config` with your configuration values from the Okta Developer Console. 
You can see all the available configuration options in the [okta-aspnet GitHub](https://github.com/okta/okta-aspnet/blob/master/README.md).
For step-by-step instructions, visit the Okta [ASP.NET MVC quickstart]. The quickstart will guide you through adding Okta login to your ASP.NET application.

Now start your server and navigate to https://localhost:44314 in your browser. 

If you see a home page that allows you to login, then things are working!  Clicking the **Log in** link will redirect you to the Okta hosted sign-in page.

You can login with the same account that you created when signing up for your Developer Org, or you can use a known username and password from your Okta Directory.

> **Notes:** If you are currently using your Developer Console, you already have a Single Sign-On (SSO) session for your Org.  You will be automatically logged into your application as the same user that is using the Developer Console.  You may want to use an incognito tab to test the flow from a blank slate.

> Because of recent changes in [Set-Cookie behavior (SameSite)](https://web.dev/samesite-cookies-explained) this code will only work properly if it's configured to use https. Check out [Work with SameSite cookies in ASP.NET](https://docs.microsoft.com/en-us/aspnet/samesite/system-web-samesite) for more details.


[OIDC Middleware Library]: https://github.com/okta/okta-aspnet
[Authorization Code Flow]: https://developer.okta.com/authentication-guide/implementing-authentication/auth-code
[OIDC Web Application Setup Instructions]: https://developer.okta.com/authentication-guide/implementing-authentication/auth-code#1-setting-up-your-application
[ASP.NET MVC quickstart]:https://developer.okta.com/quickstart/#/okta-sign-in-page/dotnet/aspnet4
