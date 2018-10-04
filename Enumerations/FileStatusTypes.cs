using System;

namespace Starship.Web.Enumerations {
    public enum FileStatusTypes {
        Unknown = 0,
        Downloading = 1,
        Uploading = 2,
        Error = 3,
        Ignored = 4,
        Uploaded = 5,
        DownloadFailed = 6,
        UploadFailed = 7
    }
}