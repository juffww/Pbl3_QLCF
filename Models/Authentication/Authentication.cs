using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace pbl3_QLCF.Models.Authentication
{
    public class Authentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //Kiểm tra Session, nếu chưa đăng nhập thì redirect về trang Login
            if (context.HttpContext.Session.GetString("TenDangNhap") == null)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {"Controller", "LoginAccess" },
                        {"Action", "Login" }
                    });
            }
        }
    }
}
