using System;
using System.IO;
using System.Threading;
using Starship.Azure.Interfaces;
using Starship.Azure.Providers;

namespace Starship.Web.Services {
    public abstract class WebAutomationService {

        protected WebAutomationService() {
            Cancel = new CancellationToken();
        }
        
        public void Run() {
            while (!Cancel.IsCancellationRequested) {
                Thread.Sleep(ProcessNext());
            }
        }
        
        public abstract TimeSpan ProcessNext();

        public CancellationToken Cancel { get; set; }

        public TimeSpan RequestDelay = TimeSpan.FromSeconds(1);

        public Func<string> GetLocalStorageRoot = Path.GetTempPath;

        public IsDataProvider DataProvider { get; set; }

        public AzureBlobStorageContainer StorageContainer { get; set; }
    }
}