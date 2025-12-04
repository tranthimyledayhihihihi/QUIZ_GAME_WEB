using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WEBB;

namespace WEBB_FE   // <== tên namespace nhớ trùng với tên project
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
