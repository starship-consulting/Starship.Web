using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Starship.Core.Data;
using Starship.Core.Storage;
using Starship.Web.Http;
using Starship.Web.Entities;
using Starship.Web.Enumerations;

namespace Starship.Web.Services {
    public class ParsingService : WebAutomationService {
        
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
                Trace.TraceError(exception.ToString());
                //DataProvider.Save(new LogMessage(Parser.GetPartition(), CurrentUrl, exception));

                if (Debugger.IsAttached)
                {
                    //throw;
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
                if(link.ShouldNavigateTo(this)) {
                    Process(link);
                }
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

            var resource = DataProvider.Get<WebResource>().ByRemoteIdentifier(partition, item.Id).Take(1).ToList().FirstOrDefault();

            if(resource != null) {
                if(resource.Score != resource.GetScore()) {
                    resource.Score = resource.GetScore();
                    DataProvider.Save(resource);
                }
            }

            if (resource == null) {
                resource = new WebResource(partition, pageUrl, item);
                resource.Score = resource.GetScore();

                if(ShouldSave == null || ShouldSave(resource)) {
                    resource = DataProvider.Save(resource);
                }
            }
            else if (resource.Status != FileStatusTypes.DownloadFailed) {
                return;
            }

            try {
                if (DownloadFiles && resource.IsDownloadable()) {

                    if(item.Data == null) {
                        item.Data = Download(resource, item.FileUrl);
                    }

                    if (item.Data != null) {
                        Upload(resource, item.Data);
                    }
                }
            }
            catch (Exception exception) {
                Trace.TraceError(exception.ToString());
                resource.Status = FileStatusTypes.Error;
                resource.StatusText = exception.ToString();
                DataProvider.Save(resource);
                throw;
            }
        }

        private byte[] Download(WebResource resource, string fileUrl) {
            Trace.TraceInformation("Downloading resource: " + fileUrl);

            resource.Status = FileStatusTypes.Downloading;
            DataProvider.Save(resource);
            
            try {
                var response = HttpHead(fileUrl);
                var downloader = Parser.GetDownloader();
                var content = downloader.Download(response, resource.Url, GetLocalStoragePath(resource));

                resource.Size = content.Size;

                var data = content.Read();

                using (var md5 = MD5.Create()) {
                    resource.Hash = BitConverter.ToString(md5.ComputeHash(data)).Replace("-", "").ToLower();
                }

                DataProvider.Save(resource);

                Trace.TraceInformation("Finished downloading file: " + fileUrl);

                return data;
            }
            catch (Exception ex) {
                Trace.TraceError("Failed downloading file: " + fileUrl + "." + Environment.NewLine + ex);

                resource.Status = FileStatusTypes.DownloadFailed;
                resource.StatusText = ex.ToString();

                DataProvider.Save(resource);
            }

            return null;
        }

        private void Upload(WebResource resource, byte[] content) {
            try {
                Trace.TraceInformation("Uploading file: " + resource.GetFilename());

                resource.Status = FileStatusTypes.Uploading;
                DataProvider.Save(resource);

                throw new Exception("No storage provider set");
                //Storage.UploadAsync(resource.GetFilename(), content).Wait();

                resource.Status = FileStatusTypes.Uploaded;
                DataProvider.Save(resource);

                Trace.TraceInformation("Finished uploading file: " + resource.GetFilename());
            }
            catch (Exception ex) {
                Trace.TraceError("Failed uploading file: " + resource + "." + Environment.NewLine + ex);

                resource.Status = FileStatusTypes.DownloadFailed;
                resource.StatusText = ex.ToString();

                DataProvider.Save(resource);
            }
            finally {
                File.Delete(GetLocalStoragePath(resource));
            }
        }

        private string GetLocalStoragePath(WebResource resource) {
            return GetLocalStorageRoot() + resource.GetFilename();
        }

        public string CurrentUrl { get; set; }
        
        public IsDataProvider DataProvider { get; set; }

        public IsFileStorageProvider Storage { get; set; }
        
        public bool DownloadFiles { get; set; }

        public Func<WebResource, bool> ShouldSave;

        private WebParser Parser { get; set; }
    }
}