using Microsoft.AspNetCore.Mvc;
using pbl3_QLCF.Models.Authentication;

namespace pbl3_QLCF.Controllers
{
    [Authentication]
    public class Staff : Controller
    {
        public IActionResult staffDashboard()
        {
            return View();
        }
    }
}
