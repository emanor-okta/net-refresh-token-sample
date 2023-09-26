using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;
using IdentityModel;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using IdentityModel.Client;

using Okta.AspNet.Abstractions;

public class RefreshTokenMiddleware : OwinMiddleware
{
    //private readonly RefreshTokenMiddlewareOptions _options;
   
    public RefreshTokenMiddleware(OwinMiddleware next/*, RefreshTokenMiddlewareOptions options*/) : base(next)
    {
        //_options = options;
    }

    public override async Task Invoke(IOwinContext context)
    {
        if (IdTokenExpired(context) && HasRefreshToken(context))
        {
            System.Diagnostics.Debug.WriteLine("IdToken About to Expire, Attempting Refresh");
            await RefreshTokens(context);
        }

        System.Diagnostics.Debug.WriteLine("Id_token is Good");
        await Next.Invoke(context);
    }

    private bool HasRefreshToken(IOwinContext context)
    {
        if (context.Authentication.User.Identity is ClaimsIdentity identity && identity.IsAuthenticated)
        {
            System.Diagnostics.Debug.WriteLine("Checking Claims");
            foreach (var claim in identity.Claims)
            {
                System.Diagnostics.Debug.WriteLine("Claim Type: " + claim.Type);
                if (claim.Type == "refresh_token")
                {
                    System.Diagnostics.Debug.WriteLine("Has RefreshToken");
                    return true;
                }
            }
            System.Diagnostics.Debug.WriteLine("Test Return True");
            return true;
        }

        System.Diagnostics.Debug.WriteLine("No RefreshToken");
        return false;
    }

    private bool IdTokenExpired(IOwinContext context)
    {
        if (context.Authentication.User?.Identity is ClaimsIdentity identity && identity.IsAuthenticated)
        {
            var idTokenString = identity.FindFirst("id_token")?.Value;
            if (idTokenString == null)
            {
                return false;
            }
            
            var refreshTokenString = identity.FindFirst("refresh_token")?.Value;
            if (refreshTokenString == null)
            {
                return false;
            }
            

            var idToken = new StrictTokenHandler().ReadToken(idTokenString);
            System.Diagnostics.Debug.WriteLine("IdToken About to Expire: " + (idToken.ValidTo <= DateTime.UtcNow.Add(TimeSpan.FromMinutes(3))));
            // Need to Add logic to update once the expiry time falls in a certain window, for now just always update
            return idToken.ValidTo <= DateTime.UtcNow.Add(TimeSpan.FromMinutes(3));
        }

        //return false;
        return true;
    }

    private async Task RefreshTokens(IOwinContext context)
    {
        var identity = (ClaimsIdentity)context.Authentication.User.Identity;
        var refreshTokenClaim = identity.FindFirst("refresh_token");
        if (refreshTokenClaim != null)
        {
            var iat = identity.FindFirst(JwtClaimTypes.IssuedAt);
            var exp = identity.FindFirst(JwtClaimTypes.Expiration);
            var accessTokenClaim = identity.FindFirst("access_token");
            var idTokenClaim = identity.FindFirst("id_token");

            identity.RemoveClaim(iat);
            identity.RemoveClaim(exp);
            identity.RemoveClaim(refreshTokenClaim);
            identity.RemoveClaim(accessTokenClaim);
            identity.RemoveClaim(idTokenClaim);
           
            using (var httpClient = new HttpClient())
            {
                var request = new RefreshTokenRequest
                {
                    RefreshToken = refreshTokenClaim.Value,
                    ClientId = "0oa2cpl777xczKzL21d7",
                    ClientSecret = "Klf03TzBqEuayATkPGy7VgTqyNDKIPsIYNd9TEKo",
                    Address = "https://oie.erikdevelopernot.com/oauth2/default/v1/token",
                };
                var tokenResponse = await httpClient.RequestRefreshTokenAsync(request);

                if (tokenResponse.IsError)
                {
                    throw new SecurityTokenException(tokenResponse.Error);
                }

                var tokenHandler = new JwtSecurityToken(tokenResponse.AccessToken);

                identity.AddClaims(new[] {
                            new Claim("access_token", tokenResponse.AccessToken),
                            new Claim("id_token", tokenResponse.IdentityToken),
                            new Claim("refresh_token", tokenResponse.RefreshToken),
                            new Claim(JwtClaimTypes.IssuedAt, tokenHandler.Payload.Iat?.ToString()),
                            new Claim(JwtClaimTypes.Expiration, tokenHandler.Payload.Exp?.ToString()),
                 });

                context.Authentication.SignIn(new AuthenticationProperties
                {
                    IssuedUtc = tokenHandler.IssuedAt,
                    ExpiresUtc = tokenHandler.ValidTo,
                },
                    identity);
            }
        }
    }
}