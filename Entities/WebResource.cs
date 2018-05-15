using System;
using System.Linq;
using Starship.Web.Enumerations;
using Starship.Web.Http;

namespace Starship.Web.Entities {
    public class WebResource {

        public WebResource() {
        }

        public WebResource(string partitionKey, string pageUrl, ContentItem item) {

            PartitionKey = partitionKey;
            RowKey = CleanUrl(item.Id);
            ParentUrl = pageUrl;
            Url = item.FileUrl;
            Extension = new Uri(item.FileUrl).AbsolutePath.Split('.').Last();
            Title = item.Title;
            Duration = item.Duration;
            Description = item.Description;
            DateAdded = item.DateAdded;
            StatusText = string.Empty;
            ContentType = item.ContentType;

            foreach (var keyword in item.Keywords) {
                var value = keyword.Trim().ToLower();

                if (string.IsNullOrEmpty(value)) {
                    continue;
                }

                Keywords += value + " ";
            }
        }

        private string CleanUrl(string url) {
            const string token = "-";

            return url
                .Replace("/", token)
                .Replace(@"\", token)
                .Replace("#", token)
                .Replace("?", token);
        }

        public string GetFilename() {
            return RowKey + "." + Extension;
        }

        public bool IsDownloadable() {
            return ContentType == FileContentTypes.Video || ContentType == FileContentTypes.Image || ContentType == FileContentTypes.Html;
        }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public string Extension { get; set; }

        public string Url { get; set; }

        public string ParentUrl { get; set; }

        public string Keywords { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Duration { get; set; }

        public DateTime? DateAdded { get; set; }

        public bool IsRemoved { get; set; }

        public long Size { get; set; }

        public string Hash { get; set; }

        public FileStatusTypes Status { get; set; }

        public string StatusText { get; set; }

        public FileContentTypes ContentType { get; set; }
    }

    public static class BaseEntityExtensions {
        public static IQueryable<WebResource> ByRemoteIdentifier(this IQueryable<WebResource> source, string partition, string identifier) {
            return source.Where(each => each.PartitionKey == partition && each.RowKey == identifier);
        }
    }
}