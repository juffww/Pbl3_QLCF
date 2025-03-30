using Microsoft.AspNetCore.Mvc;
using pbl3_QLCF.Models.Authentication;

namespace pbl3_QLCF.Controllers
{
    //[Authentication]
    public class Staff : Controller
    {
        [HttpGet]
        public IActionResult staffDashboard()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SanPham()
        {
            return View();
        }
    }
}
