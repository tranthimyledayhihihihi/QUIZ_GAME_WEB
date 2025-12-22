using System.Web.Mvc;
using System.Web.Routing;

namespace WEBB_FE
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapMvcAttributeRoutes();  // Đảm bảo rằng bạn sử dụng attribute routing nếu cần.

            // Đảm bảo đường dẫn cho khu vực Admin được cấu hình đúng
            routes.MapRoute(
                name: "Admin_Default",
                url: "Admin/{controller}/{action}/{id}",
                defaults: new { controller = "Users", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }

}
