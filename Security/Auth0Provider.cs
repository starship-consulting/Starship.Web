using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Starship.Web.Services;

namespace Starship.Web.Security {
    public static class Auth0Provider {

        public static void AddAuth0CookieAuthentication(this IServiceCollection services, Auth0Settings settings, Action<ClaimsPrincipal> onAuthenticated) {
            
            var builder = services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options => {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                //options.AccessDeniedPath = "";
            });

            AddAuth0Authentication(builder, settings, onAuthenticated);
        }

        public static void AddAuth0BearerAuthentication(this IServiceCollection services, Auth0Settings settings, Action<ClaimsPrincipal> onAuthenticated) {

            var builder = services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                options.Authority = settings.Domain;
                options.Audience = settings.Identifier;
            });

            AddAuth0Authentication(builder, settings, onAuthenticated);
        }

        private static void AddAuth0Authentication(AuthenticationBuilder builder, Auth0Settings settings, Action<ClaimsPrincipal> onAuthenticated) {

            builder.AddOpenIdConnect("Auth0", options => {

                options.Authority = "https://" + settings.Domain;
                options.ClientId = settings.ClientId;
                options.ClientSecret = settings.ClientSecret;
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

                        var logoutUri = $"https://{settings.Domain}/v2/logout?client_id={settings.ClientId}";
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