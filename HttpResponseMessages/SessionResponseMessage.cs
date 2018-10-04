using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Starship.Core.Security;
using Starship.Web.Security;

namespace Starship.Web.HttpResponseMessages {
    public class SessionResponseMessage : HttpResponseMessage {
        public SessionResponseMessage(HttpRequestMessage request, Session session, JsonSerializerSettings settings, string secretKey, string sessionKey) : base(HttpStatusCode.OK) {
            var token = new SessionToken(session, settings, secretKey);
            token.Sign();

            var serializedToken = token.Serialize();

            if (request.Headers.Contains("Origin")) {
                Headers.Add("Access-Control-Allow-Origin", request.Headers.GetValues("Origin").First());
                Headers.Add("Access-Control-Allow-Credentials", "true");
                Headers.Add("Access-Control-Allow-Methods", "GET");
                Headers.Add("Access-Control-Allow-Headers", "Content-Type");
            }

            /*Headers.AddCookies(new[] {
                new SessionCookie(sessionKey, request.RequestUri.Host.Replace("www", ""), serializedToken),
                //new SessionCookie(TopiaSettings.DomainName, serializedToken)
            });*/

            Content = new StringContent(serializedToken);
        }
    }
}