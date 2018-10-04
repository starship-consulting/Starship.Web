using System;
using System.Net.Http.Headers;

namespace Starship.Web.Security {
    /*public class SessionCookie : CookieHeaderValue {

        public SessionCookie() {
        }

        public SessionCookie(string sessionKey, string domain, string token) : base(sessionKey, token) {
            //Domain = domain;
            Expires = DateTimeOffset.Now.AddHours(1);
            Path = "/";
            HttpOnly = false;
            Value = token;
        }

        public static SessionCookie Expired(string sessionKey, string domain) {
            return new SessionCookie(sessionKey, domain, string.Empty) {
                Expires = DateTimeOffset.Now.AddDays(-1)
            };
        }

        public string Value { get; set; }
    }*/ 
}