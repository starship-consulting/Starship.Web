using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Starship.Web.Security {
    public static class Auth0Provider {

        public static void AddAuth0Authentication(this IServiceCollection services, string domain, string clientId, string clientSecret, Action<ClaimsPrincipal> onAuthenticated) {

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options => {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                //options.AccessDeniedPath = "";
            })
            .AddOpenIdConnect("Auth0", options => {

                options.Authority = $"https://{domain}";
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.ResponseType = "code";
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("email");
                options.Scope.Add("profile");
                
                options.CallbackPath = new PathString("/signin-auth0");
                options.ClaimsIssuer = "Auth0";
                options.SaveTokens = true;

                options.TokenValidationParameters = new TokenValidationParameters {
                    NameClaimType = "name"
                };

                options.Events = new OpenIdConnectEvents {
                    
                    OnTokenValidated = (context) => {
                        onAuthenticated?.Invoke(context.Principal);
                        return Task.CompletedTask;
                    },
                    
                    OnRedirectToIdentityProviderForSignOut = (context) => {

                        var logoutUri = $"https://{domain}/v2/logout?client_id={clientId}";
                        var postLogoutUri = context.Properties.RedirectUri;

                        if (!string.IsNullOrEmpty(postLogoutUri)) {

                            if (postLogoutUri.StartsWith("/")) {
                                var request = context.Request;
                                postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                            }

                            logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                        }

                        context.Response.Redirect(logoutUri);
                        context.HandleResponse();

                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}