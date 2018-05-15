using System;

namespace Starship.Web.Enumerations {
    public enum FileStatusTypes {
        Unknown = 0,
        Downloading = 1,
        Uploading = 2,
        Error = 3,
        Ignored = 4,
        Ready = 5,
        DownloadFailed = 6
    }
}