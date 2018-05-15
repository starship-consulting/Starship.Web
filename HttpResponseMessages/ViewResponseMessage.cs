using System;
using System.IO;

namespace Starship.Web.HttpResponseMessages {
    public class ViewResponseMessage : HtmlResponseMessage {
        public ViewResponseMessage(string view)
            : base(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/Views/" + view + ".html")) {
        }
    }
}
