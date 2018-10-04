using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using Starship.Core.Extensions;

namespace Starship.Web.Services {
    public abstract class HttpClientService {

        protected HttpClientService(HttpClientHandler handler = null) {
            Client = handler == null ? new HttpClient() : new HttpClient(handler);
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0");
            Client.Timeout = TimeSpan.FromMinutes(5);
        }

        private void Throttle() {
            if(LastRequest != null && !LastRequest.HasElapsed(RequestDelay, true)) {
                var sleepTime = RequestDelay - (DateTime.UtcNow - LastRequest.Value);
                Trace.TraceInformation("Throttle: " + sleepTime.TotalSeconds + " seconds.");
                Thread.Sleep(sleepTime);
            }
            
            LastRequest = DateTime.UtcNow;
        }

        public HttpResponseMessage HttpHead(string url) {
            Throttle();
            Trace.TraceInformation(DateTime.UtcNow.ToShortTimeString() + " HttpHead: " + url);
            return Client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)).Result;
        }

        public string HttpGet(string url) {
            Throttle();
            Trace.TraceInformation(DateTime.UtcNow.ToShortTimeString() + " HttpGet: " + url);
            return Client.GetStringAsync(url).Result;
        }
        
        public TimeSpan RequestDelay = TimeSpan.FromSeconds(1);

        private DateTime? LastRequest { get; set; }
        
        private HttpClient Client { get; set; }
    }
}