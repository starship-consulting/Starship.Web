using System.IO;

namespace Starship.Web.Extensions {
    public static class ControllerExtensions {

        /*public static HtmlResponseMessage View(this ApiController controller, string viewName = "", object model = null) {
            if (viewName.EndsWith(".html")) {
                return new HtmlResponseMessage(File.ReadAllText(HttpContext.Current.Server.MapPath("/Views/" + viewName)));
            }

            return controller.View(controller.GetType().Name.Replace("Controller", string.Empty), viewName, model);
        }

        public static HtmlResponseMessage View(this ApiController controller, string controllerName, string viewName, object model = null) {
            var html = new ViewRenderer(controllerName, viewName, model).GetHtml();
            return new HtmlResponseMessage(html);
        }

        public static string MapPath(this ApiController controller, string path) {
            return HttpContext.Current.Server.MapPath(path);
        }

        public static T GetSegment<T>(this ODataPath path, int index) where T : ODataPathSegment {
            return path.Segments[index].As<T>();
        }*/
    }
}