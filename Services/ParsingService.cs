using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using Starship.Web.Http;
using Starship.Web.Entities;
using Starship.Web.Enumerations;
using Starship.Azure.Interfaces;

namespace Starship.Web.Services {
    public class ParsingService : HttpClientService {
        
        public ParsingService(WebParser parser) : base(parser.GetHttpHandler()) {
            Parser = parser;
        }
        
        public override TimeSpan ProcessNext() {
            if (Cancel.IsCancellationRequested) {
                return TimeSpan.FromMilliseconds(1);
            }

            try {
                Process(Parser.GetNextPage());
            }
            catch (Exception exception) {
                DataProvider.Save(new LogMessage(Parser.GetPartition(), CurrentUrl, exception));

                if (Debugger.IsAttached) {
                    throw;
                }
            }

            return Parser.GetUpdateFrequency();
        }

        private void Process(WebPage page) {
            if (Cancel.IsCancellationRequested) {
                return;
            }

            var document = GetDocument(page);

            // Items and links should be treated as same type and implement their own handling
            foreach (var item in document.Items) {
                Save(page.Url, item);
            }

            foreach (var link in document.Links) {
                Process(link);
            }
        }

        private ParsedDocument GetDocument(WebPage page) {
            CurrentUrl = page.Url;
            
            var html = HttpGet(page.Url);

            var document = new ParsedDocument(html);
            page.Parse(document);

            return document;
        }

        private void Save(string pageUrl, ContentItem item) {
            CurrentUrl = pageUrl;

            var partition = Parser.GetPartition();

            var file = DataProvider.Get<WebResource>().ByRemoteIdentifier(partition, item.Id).FirstOrDefault();

            if (file == null) {
                file = DataProvider.Save(new WebResource(partition, pageUrl, item));
            }
            else if (file.Status != FileStatusTypes.DownloadFailed) {
                return;
            }

            try {
                if (file.IsDownloadable() && item.Data == null) {
                    item.Data = Download(file, item.FileUrl);
                }

                if (item.Data != null) {
                    Upload(file, item.Data);
                }
            }
            catch (Exception exception) {
                file.Status = FileStatusTypes.Error;
                file.StatusText = exception.ToString();
                DataProvider.Save(file);
                throw;
            }
        }

        private byte[] Download(WebResource file, string fileUrl) {
            Debug.WriteLine("Begin downloading file: " + fileUrl);

            file.Status = FileStatusTypes.Downloading;
            DataProvider.Save(file);

            try {
                var response = HttpHead(fileUrl);
                var downloader = Parser.GetDownloader();
                var content = downloader.Download(response, file.Url, GetLocalStorageRoot());

                file.Size = content.Size;

                var data = content.Read();

                using (var md5 = MD5.Create()) {
                    file.Hash = BitConverter.ToString(md5.ComputeHash(data)).Replace("-", "").ToLower();
                }

                DataProvider.Save(file);

                Debug.WriteLine("Finish downloading file: " + fileUrl);

                return data;
            }
            catch (Exception ex) {
                Debug.WriteLine("Failed downloading file: " + fileUrl);

                file.Status = FileStatusTypes.DownloadFailed;
                file.StatusText = ex.ToString();

                DataProvider.Save(file);
            }

            return null;
        }

        private void Upload(WebResource file, byte[] content) {
            Debug.WriteLine("Begin uploading file: " + file.GetFilename());

            file.Status = FileStatusTypes.Uploading;
            DataProvider.Save(file);

            StorageContainer.Upload(file.GetFilename(), content);

            file.Status = FileStatusTypes.Ready;
            DataProvider.Save(file);

            Debug.WriteLine("Finish uploading file: " + file.GetFilename());
        }

        public string CurrentUrl { get; set; }
        
        private WebParser Parser { get; set; }
    }
}