using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Starship.Web.Security {
    public static class SessionFactory {

        public static Session Authenticate(SessionCredentials credentials) {
            var session = Session.Lookup(credentials);

            if (session != null) {
                if (!Cryptography.Validate(AppConfiguration.SecretKey, session.GetPassword(), credentials.Password)) {
                    //return null;
                }

                session.IsAuthenticated = true;
            }

            return session;
        }
        
        /*public static Session GetSession(HttpCookie cookie) {
            if (cookie != null) {
                return GetSession(cookie.Value);
            }

            return new Session();
        }*/

        /*public static Session GetSession(string token) {
            if (!token.IsNull()) {
                token = HttpUtility.UrlDecode(token);
                var securityToken = JsonConvert.DeserializeObject<SecurityToken>(token);

                if (securityToken != null && securityToken.Validate()) {
                    var session = JsonConvert.DeserializeObject<Session>(JsonConvert.SerializeObject(securityToken.Data));
                    return session;
                }
            }
            
            return new Session();
        }*/

        public static Session GetCurrentSession(ISession httpContextSession) {
            if(httpContextSession.IsAvailable) {
                var session = httpContextSession.GetString(SessionKey);

                if(!session.IsNull()) {
                    return JsonConvert.DeserializeObject<Session>(session);
                }
            }

            return null;
        }

        public static AuthenticationResult ProcessResponse(HttpRequest request, HttpResponse response, Session session, ISession httpContextSession) {
            
            //var serializedToken = GetSessionToken(session);
            var token = JsonConvert.SerializeObject(session);
            var uri = request.GetUri();

            var domain = uri.Scheme + "://" + uri.Authority + "/";

            /*response.Cookies.Append(SessionKey, serializedToken, new CookieOptions {
                Domain = uri.Host,
                Expires = DateTimeOffset.UtcNow.Add(AppConfiguration.SessionCookieDuration),
                Path = "/"
            });*/
            
            httpContextSession.SetString(SessionKey, token);

            var result = new AuthenticationResult {
                Token = token,
                Session = session
            };
            
            response.Headers.Add("Access-Control-Allow-Origin", domain);
            response.Headers.Add("Access-Control-Allow-Credentials", "true");

            return result;
        }

        /*public static string GetSessionToken(Session session = null) {
            if (session == null) {
                session = Session.Current;
            }

            if (!session.IsAuthenticated) {
                return string.Empty;
            }

            var token = new SecurityToken(session);
            token.Sign();

            return token.Serialize();
        }*/

        public static HttpCookie GetLogoutCookie() {
            var cookie = new HttpCookie(SessionKey, string.Empty);

            cookie.Expires = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1));

            return cookie;
        }

        private static HttpCookie GetCookieState(List<HttpCookie> cookies) {
            return cookies.FirstOrDefault(each => each.Key == SessionKey);
        }
        
        /*public static void Release() {
            HttpContext.Current.Items[SessionKey] = null;
        }

        public static Session Set(Session session) {
            HttpContext.Current.Items[SessionKey] = session;
            return session;
        }

        public static Session Get() {
            if (HttpContext.Current.Items.Contains(SessionKey)) {
                return HttpContext.Current.Items[SessionKey] as Session;
            }

            return new Session();
        }*/
        
        private const string SessionKey = "session";
    }
}