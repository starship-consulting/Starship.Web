using System;

namespace Starship.Web.QueryModels {
    public class EventQueryParameters {
        
        public string Type { get; set; }

        public string Name { get; set; }

        public bool IncludeSources { get; set; }

        public string Filter { get; set; }

        public int Top { get; set; }

        public string Partition { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }
}