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
                if (TempData["SuccessMessage"] != null)
                {
                    ViewBag.SuccessMessage = TempData["SuccessMessage"];
                }
            }
            return View();
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
                    HttpContext.Session.SetString("Ten", u.HoTen.ToString());
                    HttpContext.Session.SetString("maNV", u.MaNv.ToString());
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
        private bool CheckEmail(string email)
        {
            var user = db.NguoiDungs.FirstOrDefault(e => e.Email == email);
            return user != null;

        }
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

            if(!CheckEmail(email))
            {
                ViewBag.Message = "No user database matched";
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
                ViewBag.Message = "Failed to send email. Please try again";
                return View("VerifyCode");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ResendVerificationCode()
        {
            // Get the email from session
            string email = HttpContext.Session.GetString("ChangePWEmail");

            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Email not found" });
            }

            // Generate new code
            Random rand = new Random();
            string code = rand.Next(100000, 999999).ToString();

            // Save the new code in session
            HttpContext.Session.SetString("ChangePWCode", code);

            // Send the email
            string subject = "Password Recovery Code";
            string message = $"Your verification code is: <strong>{code}</strong>";

            try
            {
                await _emailSender.SendEmailAsync(email, subject, message);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false, message = "Failed to send email" });
            }
        }
        [HttpPost]
        public IActionResult VerifyCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                ModelState.AddModelError("", "Verification code is required");
                ViewBag.ErrorMessage = "Verification code is required";
                return View();
            }

            // Lấy mã từ session
            string savedCode = HttpContext.Session.GetString("ChangePWCode");

            if (savedCode == null || savedCode != code)
            {
                ModelState.AddModelError("", "Invalid verification code");
                ViewBag.ErrorMessage = "Invalid verification code";
                return View();
            }

            // Nếu code đúng, chuyển hướng đến trang đổi mật khẩu với state "Code sent"
            return RedirectToAction("ResetPassword", new { state = "Change Password" });
        }
        [HttpGet]
        public IActionResult VerifyCode()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ResetPassword(string state)
        {
            var model = new ChangePasswordViewModel
            {
                State = state
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult ResetPassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string email = HttpContext.Session.GetString("ChangePWEmail");
            if (model.newPassword != model.confirmPassword)
            {
                ModelState.AddModelError("confirmPassword", "Mật khẩu xác nhận không khớp với mật khẩu mới");
                ViewBag.ErrorMessage = "Mật khẩu xác nhận không khớp với mật khẩu mới";
                return View(model);
            }

            var user = db.NguoiDungs.FirstOrDefault(x => x.Email == email);
            if (user != null)
            {
                user.MatKhau = model.newPassword;
                db.SaveChanges();
                HttpContext.Session.Remove("ChangePWCode");
                HttpContext.Session.Remove("ChangePWEmail");
                TempData["SuccessMessage"] = "Đã thay đổi mật khẩu thành công";
                ViewBag.SuccessMessage = "Đã thay đổi mật khẩu thành công";
                return RedirectToAction("Login", "LoginAccess");
            }
            else
            {
                ModelState.AddModelError("", "User not found");
                return View(model);
            }
        }
    }
}