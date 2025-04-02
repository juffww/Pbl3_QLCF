using Microsoft.AspNetCore.Mvc;
using pbl3_QLCF.Data;
using System.Net.Mail;
using System.Net;
using pbl3_QLCF.Models;
using pbl3_QLCF.Service;
using pbl3_QLCF.ViewModels;

namespace pbl3_QLCF.Controllers
{
    public class LoginAccess : Controller
    {
        private readonly Pbl3Context db = new Pbl3Context();
        private readonly IMyEmailSender _emailSender;
        public LoginAccess(Pbl3Context db, IMyEmailSender emailSender)
        {
            this.db = db;
            _emailSender = emailSender;
        }

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
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                var query = db.NguoiDungs.Where(x => x.TenDangNhap.Equals(user.TenDangNhap) &&
                                                    x.MatKhau.Equals(user.MatKhau));

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
                    if (u.ChucVu.Equals("Quản lý"))
                    {
                        return RedirectToAction("magDashboard", "Manager");
                    }
                    else
                    {
                        return RedirectToAction("staffDashBoard", "Staff");
                    }
                }
                else
                {
                    ViewBag.LoginError = "Sai tên đăng nhập hoặc mật khẩu!";
                    return View();
                }
            }
            return View();
        }
        //private void CheckEmail(string email)
        //{
        //    string EmailDb = db.NguoiDungs.FirstOrDefault(e => e.Email == email).ToString();
        //}
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Email is required");
                return View();
            }

            // Tạo mã xác nhận
            Random rand = new Random();
            string code = rand.Next(100000, 999999).ToString();

            // Lưu mã vào session
            HttpContext.Session.SetString("ChangePWCode", code);
            HttpContext.Session.SetString("ChangePWEmail", email);

            // Gửi email
            string subject = "Password Recovery Code";
            string message = $"Your verification code is: <strong>{code}</strong>";

            try
            {
                await _emailSender.SendEmailAsync(email, subject, message);
                return RedirectToAction("VerifyCode", new { state = "Code sent" });
            }
            catch
            {
                ModelState.AddModelError("", "Failed to send email. Please try again.");
                return View("VerifyCode");
            }
        }
        [HttpPost]
        public IActionResult VerifyCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                ModelState.AddModelError("", "Verification code is required");
                return View();
            }

            // Lấy mã từ session
            string savedCode = HttpContext.Session.GetString("ChangePWCode");

            if (savedCode == null || savedCode != code)
            {
                ModelState.AddModelError("", "Invalid verification code");
                return View();
            }

            // Nếu code đúng, chuyển hướng đến trang đổi mật khẩu với state "Code sent"
            return RedirectToAction("ResetPassword", new { state = "Code sent" });
        }
        [HttpGet]
        public IActionResult VerifyCode()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ResetPassWord(string state)
        {
            var model = new ChangePasswordViewModel
            {
                State = state
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.State == "Code sent")
            {
                string savedCode = HttpContext.Session.GetString("ChangePWCode");
                if (savedCode != model.ConfirmCode)
                {
                    ModelState.AddModelError("ConfirmCode", "Invalid verification code");
                    return View(model);
                }

                model.State = "Change Password";
                return View(model);
            }
            else if (model.State == "Change Password")
            {
                // Xử lý đổi mật khẩu ở đây
                // Lấy email từ session
                string email = HttpContext.Session.GetString("ChangePWEmail");

                // TODO: Cập nhật mật khẩu mới trong database

                // Xóa session
                HttpContext.Session.Remove("ChangePWCode");
                HttpContext.Session.Remove("ChangePWEmail");

                return RedirectToAction("Login"); // Chuyển hướng đến trang đăng nhập
            }

            return View(model);
        }
    }
}