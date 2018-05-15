using System;
using System.Net.Http;
using Starship.Core.Http;

namespace Starship.Web.Http {
    public abstract class WebParser {

        public abstract string GetPartition();

        public virtual TimeSpan GetUpdateFrequency() {
            return TimeSpan.FromMinutes(5);
        }

        public virtual HttpClientHandler GetHttpHandler() {
            return new HttpClientHandler();
        }

        public virtual IDownloader GetDownloader() {
            return new WebClientDownloader();
        }

        public abstract WebPage GetNextPage();
    }
}