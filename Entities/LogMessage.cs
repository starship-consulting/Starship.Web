using System;
using Starship.Web.Logging;

namespace Starship.Web.Entities {
    public class LogMessage {
        public LogMessage() {
        }

        public LogMessage(string partition, string context, Exception ex) {
            PartitionKey = partition;
            Context = context;
            Message = ex.ToString();
            Type = LogTypes.Error;
        }
        
        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public string Message { get; set; }

        public string Context { get; set; }

        public LogTypes Type { get; set; }
    }
}