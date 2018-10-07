using System.Net.Http;
using Starship.Core.Http;

namespace Starship.Web.Http {
    public interface IDownloader {
        FileDetails Download(HttpResponseMessage response, string url, string rootPath);
    }
}
