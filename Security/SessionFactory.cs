using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Starship.Core.Security;

namespace Starship.Web.Security {
    public static class SessionFactory {

        public static Session FromRequest(string sessionKey, HttpRequestMessage request) {
            var cookie = GetSessionCookie(sessionKey, request);

            if (cookie == null) {
                return null;
            }

            var token = JsonConvert.DeserializeObject<SessionToken>(cookie.Value);

            return token.Validate() ? token.Data : null;
        }

        private static CookieState GetSessionCookie(string sessionKey, IEnumerable<CookieState> cookies) {
            return cookies.FirstOrDefault(each => each.Name == sessionKey);
        }

        private static CookieState GetSessionCookie(string sessionKey, HttpRequestMessage request) {
            return GetSessionCookie(sessionKey, request.Headers.GetCookies().SelectMany(each => each.Cookies).ToList());
        }
    }
}