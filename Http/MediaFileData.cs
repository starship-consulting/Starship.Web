using System;

namespace Starship.Web.Http {
    public class MediaFileData {

        public MediaFileData() {
        }

        public MediaFileData(byte[] buffer) {
            Buffer = buffer;
            FileName = string.Empty;
            MediaType = string.Empty;
        }

        public MediaFileData(byte[] buffer, string mediaType, string fileName) {
            Buffer = buffer;
            MediaType = mediaType;
            FileName = fileName.Replace("\"", "");
        }

        public string FileName { get; set; }

        public string MediaType { get; set; }

        public byte[] Buffer { get; set; }
    }
}