using Microsoft.AspNetCore.Mvc;
using pbl3_QLCF.Data;
using System.Net.Mail;
using System.Net;
using pbl3_QLCF.Models;

namespace pbl3_QLCF.Controllers
{
    public class LoginAccess : Controller
    {
        private readonly Pbl3Context db = new Pbl3Context();

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public IActionResult Login(NguoiDung user, string userRole)
        {
            //Xu ly du lieu
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                // Query based on role
                var query = db.NguoiDungs.Where(x => x.TenDangNhap.Equals(user.TenDangNhap) &&
                                                    x.MatKhau.Equals(user.MatKhau));

                // Add role filter based on your database structure
                // Assuming you have a "ChucVu" field in your NguoiDung table
                if (userRole == "manager")
                {
                    query = query.Where(x => x.ChucVu.Equals("Quản lý"));
                }
                else
                {
                    query = query.Where(x => x.ChucVu.Equals("Nhân viên"));
                }

                var u = query.FirstOrDefault();

                if (u != null)
                {
                    HttpContext.Session.SetString("TenDangNhap", u.TenDangNhap.ToString());
                    HttpContext.Session.SetString("UserRole", u.ChucVu.ToString());
                    // Redirect based on role
                    if (u.ChucVu.Equals("Quản lý"))
                    {
                        // Ensure this method exists in the ManagerController
                        return RedirectToAction("magDashboard", "Manager");
                    }
                    else
                    {
                        // Ensure this method exists in the StaffController
                        return RedirectToAction("staffDashBoard", "Staff");
                    }
                }
                else
                {
                    // Thêm thông báo lỗi vào ViewBag
                    ViewBag.LoginError = "Sai tên đăng nhập hoặc mật khẩu!";
                    return View();
                }
            }
            return View();
        }       
    }
}