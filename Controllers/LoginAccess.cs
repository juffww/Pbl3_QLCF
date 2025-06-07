using Microsoft.AspNetCore.Mvc;
using pbl3_QLCF.Data;
using System.Net.Mail;
using System.Net;
using pbl3_QLCF.Models;
using pbl3_QLCF.Service;
using pbl3_QLCF.ViewModels;
using pbl3_QLCF.Interface;

namespace pbl3_QLCF.Controllers
{
    public class LoginAccess : Controller
    {
        private readonly Pbl3Context db = new Pbl3Context();
        private readonly IMyEmailSender _emailSender;
        private CustomerService customerService;
        public LoginAccess(Pbl3Context db, IMyEmailSender emailSender, CustomerService customerService)
        {
            this.db = db;
            _emailSender = emailSender;
            this.customerService = customerService;
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
        public IActionResult Login(NguoiDung user)
        {
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                var u = db.NguoiDungs.FirstOrDefault(x => x.TenDangNhap.Equals(user.TenDangNhap));

                if (u != null && VerifyPassword(user.MatKhau, u.MatKhau))
                {
                    HttpContext.Session.SetString("TenDangNhap", u.TenDangNhap.ToString());
                    HttpContext.Session.SetString("UserRole", u.ChucVu.ToString());
                    HttpContext.Session.SetString("Ten", u.HoTen.ToString());
                    HttpContext.Session.SetString("maNV", u.MaNv.ToString());

                    if (u.ChucVu.Equals("Quản lý"))
                    {
                        customerService.UpdateCustomerTypes();
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
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
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
            Random rand = new Random();
            string code = rand.Next(100000, 999999).ToString();

            HttpContext.Session.SetString("ChangePWCode", code);
            HttpContext.Session.SetString("ChangePWEmail", email);

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
            string email = HttpContext.Session.GetString("ChangePWEmail");

            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Email not found" });
            }

            Random rand = new Random();
            string code = rand.Next(100000, 999999).ToString();

            HttpContext.Session.SetString("ChangePWCode", code);

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

            string savedCode = HttpContext.Session.GetString("ChangePWCode");

            if (savedCode == null || savedCode != code)
            {
                ModelState.AddModelError("", "Invalid verification code");
                ViewBag.ErrorMessage = "Invalid verification code";
                return View();
            }

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
            if (!IsStrongPassword(model.newPassword))
            {
                ModelState.AddModelError("newPassword", "Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ hoa, chữ thường và số");
                ViewBag.ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ hoa, chữ thường và số";
                return View(model);
            }

            var user = db.NguoiDungs.FirstOrDefault(x => x.Email == email);
            if (user != null)
            {
                user.MatKhau = HashPassword(model.newPassword);
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
        private bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying password: {ex.Message}");
                return false;
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasNumber = password.Any(char.IsDigit);

            return hasUpper && hasLower && hasNumber;
        }
    }
}