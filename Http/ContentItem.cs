using System;
using System.Collections.Generic;
using Starship.Web.Enumerations;

namespace Starship.Web.Http {
    public class ContentItem {
        public ContentItem() {
            Keywords = new List<string>();
        }

        public string Id { get; set; }

        public byte[] Data { get; set; }

        public string SourceUrl { get; set; }

        public string FileUrl { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Duration { get; set; }

        public DateTime DateAdded { get; set; }

        public List<string> Keywords { get; set; }

        public FileContentTypes ContentType { get; set; }
    }
}
