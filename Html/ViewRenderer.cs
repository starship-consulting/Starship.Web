

//using System.Web.Mvc;
//using System.Web.Routing;

namespace Starship.Web.Html {
    /*public class ViewRenderer {
        public ViewRenderer() {
        }

        public ViewRenderer(string controllerName, string viewName, object model) {
            ControllerName = controllerName;
            ViewName = viewName;
            Model = model;
        }

        public string GetHtml() {
            using (var writer = new StringWriter()) {
                var routeData = new RouteData();
                routeData.Values.Add("controller", ControllerName);
                var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://localhost/", null), new HttpResponse(null))), routeData, new ControllerStub());
                var razorViewEngine = new RazorViewEngine();
                var razorViewResult = razorViewEngine.FindView(fakeControllerContext, ViewName, "", false);

                var viewContext = new ViewContext(fakeControllerContext, razorViewResult.View, new ViewDataDictionary(Model), new TempDataDictionary(), writer);
                razorViewResult.View.Render(viewContext, writer);
                return writer.ToString();
            }
        }

        protected class ControllerStub : ControllerBase {
            protected override void ExecuteCore() {
            }
        }

        public string ControllerName { get; set; }

        public string ViewName { get; set; }

        public object Model { get; set; }
    }*/
}