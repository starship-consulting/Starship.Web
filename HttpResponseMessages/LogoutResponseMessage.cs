﻿using System.Net.Http;

namespace Starship.Web.HttpResponseMessages {
    public class LogoutResponseMessage : HttpResponseMessage {
        public LogoutResponseMessage(HttpRequestMessage request, string sessionKey, string domainName) {
            /*Headers.AddCookies(new[] {
                SessionCookie.Expired(sessionKey, request.RequestUri.Host),
                SessionCookie.Expired(sessionKey, domainName)
            });*/
        }
    }
}