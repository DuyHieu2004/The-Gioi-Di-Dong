using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TheGioiDiDong.Controllers
{
    public class AdminBaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Kiểm tra nếu chưa đăng nhập hoặc quyền không phải Admin
            if (Session["QuyenHan"] == null || Session["QuyenHan"].ToString() != "Admin")
            {
                // Đá về trang Đăng nhập
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Account", action = "Login" })
                );
            }
            base.OnActionExecuting(filterContext);
        }
    }
}