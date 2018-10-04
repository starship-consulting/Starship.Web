using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Starship.Web.Services {
    public abstract class WebAutomationService : HttpClientService {

        protected WebAutomationService(HttpClientHandler handler = null) : base(handler) {
            Cancel = new CancellationToken();
        }

        public void RunAsync() {
            var thread = new Thread(Run);
            thread.Start();
        }
        
        public void Run() {
            while (!Cancel.IsCancellationRequested) {
                Thread.Sleep(ProcessNext());
            }
        }
        
        public abstract TimeSpan ProcessNext();

        public CancellationToken Cancel { get; set; }
        
        public Func<string> GetLocalStorageRoot = Path.GetTempPath;
    }
}