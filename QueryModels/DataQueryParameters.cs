using System;

namespace Starship.Web.QueryModels {
    public class DataQueryParameters {

        public DateTime? IncludeInvalidated { get; set; }

        public int Top { get; set; }
    }
}