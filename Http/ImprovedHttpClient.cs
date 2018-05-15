using System;
using System.IO;
using System.Net;
using System.Net.Http;
using CsQuery;
using Starship.Core.Extensions;
using Starship.Core.Http;

namespace Starship.Web.Http {
    public class ImprovedHttpClient : IDisposable {

        public ImprovedHttpClient(bool spoofUserAgent = true) {

            Handler = new HttpClientHandler {
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };

            Client = new HttpClient(Handler) {
                Timeout = TimeSpan.FromMinutes(5)
            };

            if (spoofUserAgent) {
                Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0");
            }
        }

        public FileDetails Download(string url, IDownloader downloader) {
            var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)).Result;
            var path = Path.GetTempPath() + Guid.NewGuid();
            return downloader.Download(response, url, path);
        }

        public string Get(string url) {
            return Client.GetStringAsync(url).GetResult();
        }

        public CQ GetDocument(string url) {
            return CQ.CreateDocument(Get(url));
        }

        public ImprovedHttpClient AddCookie(Cookie cookie) {
            Handler.CookieContainer.Add(cookie);
            return this;
        }

        public void Dispose() {
            if (Client != null) {
                Client.Dispose();
                Client = null;
            }
        }

        private HttpClient Client { get; set; }

        private HttpClientHandler Handler { get; set; }
    }
}