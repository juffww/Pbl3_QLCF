using Microsoft.AspNetCore.Mvc;
using pbl3_QLCF.Data;
using pbl3_QLCF.Models.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using pbl3_QLCF.ViewModels;

namespace pbl3_QLCF.Controllers
{
    [Authentication]
    public class Staff : Controller
    {
        private readonly Pbl3Context _context;

        public Staff(Pbl3Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult SanPham(string loaiSanPham = null, string search = null)
        {
            var donHang = GetOrCreateDonHang();

            var cartProductIds = donHang.ChiTietDonHangs.Select(c => c.MaMon).ToList();

            var filteredProducts = string.IsNullOrEmpty(search)
                ? GetThucDons(loaiSanPham)
                : _context.ThucDons.Where(t => (t.TenMon.Contains(search) || t.TenLoai.Contains(search)) &&
                                      (string.IsNullOrEmpty(loaiSanPham) || t.TenLoai == loaiSanPham)).ToList();

            var filteredProductIds = filteredProducts.Select(p => p.MaMon).ToList();

            var cartProducts = _context.ThucDons
                .Where(t => cartProductIds.Contains(t.MaMon))
                .ToList();

            string searchPhone = TempData["SearchPhone"] as string ?? "";
            int diemTichLuy = 0;

            if (!string.IsNullOrEmpty(searchPhone))
            {
                var customer = _context.KhachHangs.FirstOrDefault(c => c.Sdt == searchPhone);
                if (customer != null)
                {
                    diemTichLuy = customer.DiemTichLuy ?? 0;
                }
            }
            var model = new SanPhamViewModel
            {
                ThucDons = filteredProducts,
                Cart = cartProducts,
                DonHangHienTai = donHang,
                ProductTypes = GetDistinctProductTypes(),
                SearchString = search,
                DTL = diemTichLuy
            };

            TempData["SelectedCategory"] = loaiSanPham;
            TempData["SearchString"] = search;

            return View(model);
        }

        [HttpPost]
        public IActionResult TimKiem(string search)
        {
            return RedirectToAction("SanPham", new { loaiSanPham = TempData["SelectedCategory"]?.ToString(), search });
        }

        [HttpPost]
        public IActionResult ThemVaoDonHang(string maMon, int soLuong = 1, string ghiChu = "")
        {
            var donHang = GetOrCreateDonHang(); 

            var thucDon = _context.ThucDons.FirstOrDefault(m => m.MaMon == maMon);
            if (thucDon == null)
            {
                return RedirectToAction("SanPham");
            }

            var chiTiet = donHang.ChiTietDonHangs.FirstOrDefault(c => c.MaMon == maMon);
            if (chiTiet != null)
            {
                chiTiet.SoLuong += soLuong;
                if (!string.IsNullOrEmpty(ghiChu))
                {
                    chiTiet.GhiChu = ghiChu;
                }
            }
            else
            {
                var newItem = new ChiTietDonHang
                {
                    MaDh = donHang.MaDh,
                    MaMon = thucDon.MaMon,
                    SoLuong = soLuong,
                    GiaBan = thucDon.GiaSp,
                    GhiChu = ghiChu
                };

                donHang.ChiTietDonHangs.Add(newItem);
            }

            CapNhatTongTien(donHang);

            HttpContext.Session.SetObjectAsJson("DonHangHienTai", donHang);

            return RedirectToAction("SanPham", new
            {
                loaiSanPham = TempData["SelectedCategory"]?.ToString(),
                search = TempData["SearchString"]?.ToString()
            });
        }

        [HttpPost]
        public IActionResult CapNhatSoLuong(string maMon, int soLuong)
        {
            var donHang = HttpContext.Session.GetObjectFromJson<DonHang>("DonHangHienTai");
            if (donHang == null)
            {
                return RedirectToAction("SanPham");
            }

            var chiTiet = donHang.ChiTietDonHangs.FirstOrDefault(c => c.MaMon == maMon);
            if (chiTiet != null)
            {
                if (soLuong <= 0)
                {
                    donHang.ChiTietDonHangs.Remove(chiTiet);
                }
                else
                {
                    chiTiet.SoLuong = soLuong;
                }

                CapNhatTongTien(donHang);
                HttpContext.Session.SetObjectAsJson("DonHangHienTai", donHang);
            }

            return RedirectToAction("SanPham");
        }

        [HttpPost]
        public IActionResult ThemGhiChu(string maMon, string ghiChu)
        {
            var donHang = HttpContext.Session.GetObjectFromJson<DonHang>("DonHangHienTai");
            if (donHang == null)
            {
                return RedirectToAction("SanPham");
            }

            var chiTiet = donHang.ChiTietDonHangs.FirstOrDefault(c => c.MaMon == maMon);
            if (chiTiet != null)
            {
                chiTiet.GhiChu = ghiChu;
                HttpContext.Session.SetObjectAsJson("DonHangHienTai", donHang);
            }

            return RedirectToAction("SanPham");
        }

        [HttpPost]
        public IActionResult XoaSanPham(string maMon)
        {
            var donHang = HttpContext.Session.GetObjectFromJson<DonHang>("DonHangHienTai");
            if (donHang == null)
            {
                return RedirectToAction("SanPham");
            }

            var chiTiet = donHang.ChiTietDonHangs.FirstOrDefault(c => c.MaMon == maMon);
            if (chiTiet != null)
            {
                donHang.ChiTietDonHangs.Remove(chiTiet);
                CapNhatTongTien(donHang);
                HttpContext.Session.SetObjectAsJson("DonHangHienTai", donHang);
            }

            return RedirectToAction("SanPham");
        }

        [HttpPost]
        public IActionResult XoaDonHang()
        {
            var donHangMoi = CreateNewOrder();
            HttpContext.Session.SetObjectAsJson("DonHangHienTai", donHangMoi);
    
            TempData["SuccessMessage"] = "Đơn hàng đã được xóa!";
            return RedirectToAction("SanPham");
        }
        [HttpPost]
        public IActionResult HoanTatDonHang(string tenKhachHang, string soDienThoai, string ghiChuDonHang, string ban = null, bool usePoints = true, int pointsToUse = 0)
        {
            var donHang = HttpContext.Session.GetObjectFromJson<DonHang>("DonHangHienTai");
            if (donHang == null || !donHang.ChiTietDonHangs.Any())
            {
                return RedirectToAction("SanPham");
            }

            string? maKh = null;
            if (string.IsNullOrWhiteSpace(tenKhachHang) || string.IsNullOrWhiteSpace(soDienThoai))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin khách hàng.";
                return RedirectToAction("SanPham");
            }
            if (!string.IsNullOrEmpty(tenKhachHang) && !string.IsNullOrEmpty(soDienThoai))
            {
                var khachHang = _context.KhachHangs.FirstOrDefault(k => k.Sdt == soDienThoai);
                if (khachHang == null)
                {
                    khachHang = new KhachHang
                    {
                        MaKh = "KH" + soDienThoai,
                        TenKh = tenKhachHang,
                        Sdt = soDienThoai
                    };
                    _context.KhachHangs.Add(khachHang);
                    _context.SaveChanges();
                }

                maKh = khachHang.MaKh;
                if (usePoints && khachHang.DiemTichLuy > 0 && pointsToUse > 0)
                {
                    int availablePoints = khachHang.DiemTichLuy ?? 0;
                    int pointsToDeduct = Math.Min(availablePoints, pointsToUse);

                    int discountAmount = pointsToDeduct * 1000;

                    donHang.TongTien -= discountAmount;
                    if (donHang.TongTien < 0) donHang.TongTien = 0;

                    khachHang.DiemTichLuy -= pointsToDeduct;
                }

                int newPoints = (int)(donHang.TongTien / 10000);
                khachHang.DiemTichLuy += newPoints;
                _context.SaveChanges();
            }
            
            var maNV = HttpContext.Session.GetString("maNV");
            var orderToSave = new DonHang
            {
                MaDh = donHang.MaDh,
                MaKh = maKh,
                MaNv = maNV,
                ThoiGianDat = DateTime.Now,
                TongTien = donHang.TongTien,
                ThanhToan = "Đã thanh toán",
                TrangThaiDh = "Mới",
                MaBan = ban
            };

            _context.DonHangs.Add(orderToSave);
            _context.SaveChanges();

            foreach (var item in donHang.ChiTietDonHangs)
            {
                var chiTiet = new ChiTietDonHang
                {
                    MaDh = donHang.MaDh,
                    MaMon = item.MaMon,
                    SoLuong = item.SoLuong,
                    GiaBan = item.GiaBan,
                    GhiChu = item.GhiChu
                };
                _context.ChiTietDonHangs.Add(chiTiet);
            }

            _context.SaveChanges();
            var donHangMoi = CreateNewOrder();
            HttpContext.Session.SetObjectAsJson("DonHangHienTai", donHangMoi);
            TempData["SuccessMessage"] = "Đơn hàng đã được tạo thành công!";
            return RedirectToAction("SanPham");
        }

        private List<ThucDon> GetThucDons(string loaiSanPham)
        {
            if (string.IsNullOrEmpty(loaiSanPham))
            {
                return _context.ThucDons.Where(t => t.TinhTrang == true).ToList();
            }
            else
            {
                return _context.ThucDons.Where(t => t.TenLoai == loaiSanPham && t.TinhTrang == true).ToList();
            }
        }

        private DonHang GetOrCreateDonHang()
        {
            var donHang = HttpContext.Session.GetObjectFromJson<DonHang>("DonHangHienTai");
            if (donHang == null)
            {
                donHang = CreateNewOrder();
            }

            if (donHang.ChiTietDonHangs == null)
            {
                donHang.ChiTietDonHangs = new List<ChiTietDonHang>();
            }

            HttpContext.Session.SetObjectAsJson("DonHangHienTai", donHang);
            return donHang;
        }

        private DonHang CreateNewOrder()
        {
            return new DonHang
            {
                MaDh = GenerateNewOrderId(),
                ThoiGianDat = DateTime.Now,
                TongTien = 0,
                ThanhToan = "Chưa thanh toán",
                TrangThaiDh = "Mới",
                ChiTietDonHangs = new List<ChiTietDonHang>(),
            };
        }

        private string GenerateNewOrderId()
        {
            string prefix = "DH";

            var latestOrder = _context.DonHangs
                .Where(o => o.MaDh.StartsWith(prefix))
                .OrderByDescending(o => o.MaDh)
                .FirstOrDefault();

            if (latestOrder == null)
            {
                return prefix + "001";
            }
            else
            {
                try
                {
                    string currentNumber = latestOrder.MaDh.Substring(prefix.Length);
                    if (int.TryParse(currentNumber, out int currentVal))
                    {
                        int nextNumber = currentVal + 1;
                        return prefix + nextNumber.ToString("D3");
                    }
                    else
                    {
                        return prefix + "001";
                    }
                }
                catch
                {
                    return prefix + "001";
                }
            }
        }

        private void CapNhatTongTien(DonHang donHang)
        {
            int tongTien = 0;
            foreach (var item in donHang.ChiTietDonHangs)
            {
                tongTien += (int)(item.GiaBan * item.SoLuong);
            }
            donHang.TongTien = tongTien;

            _context.SaveChanges();
        }

        public List<string> GetDistinctProductTypes()
        {
            return _context.ThucDons
                .Select(t => t.TenLoai)
                .Distinct()
                .Where(t => t != null)
                .ToList();
        }

        //------------------DonHang---------
        [HttpGet]
        public IActionResult DonHang(string page = "1", string category = "all")
        {
            IQueryable<DonHang> query = _context.DonHangs;
            if(category != "all")
            {
                switch(category)
                {
                    case "Mới": query = query.Where(q => q.TrangThaiDh == "Mới"); break;
                    case "Đang xử lý": query = query.Where(q => q.TrangThaiDh == "Đang xử lý"); break;
                    case "Hoàn thành": query = query.Where(q => q.TrangThaiDh == "Hoàn thành"); break;
                }
            }
            var donHang = query.Include(d => d.MaKhNavigation)
                                .Include(d => d.ChiTietDonHangs)
                                .ThenInclude(d => d.MaMonNavigation)
                                .OrderByDescending(d => d.ThoiGianDat)
                                .ToList();
            ViewBag.currentPage = page;
            ViewBag.currentCategory = category;
            return View(donHang);
        }
        [HttpPost]
        public IActionResult capNhatTrangThai(string id)
        {
            var order = _context.DonHangs.FirstOrDefault(d => d.MaDh == id);
            if (order == null)
            {
                return RedirectToAction("XuLyDon");
            }
            order.TrangThaiDh = "Hoàn thành";
            _context.SaveChanges();
            return RedirectToAction("XuLyDon", new { id = id});
        }
        [HttpGet]
        public IActionResult XuLyDon(string id)
        {
            var order = _context.DonHangs
                .Include(o => o.ChiTietDonHangs)
                .ThenInclude(c => c.MaMonNavigation)
                .Include(o => o.MaBanNavigation) 
                .FirstOrDefault(o => o.MaDh == id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.TrangThaiDh == "Mới")
            {
                order.TrangThaiDh = "Đang xử lý";
                _context.SaveChanges();
            }

            var KH = _context.KhachHangs.FirstOrDefault(kh => kh.MaKh == order.MaKh);
            string? tenNhanVien = null;
            if (!string.IsNullOrEmpty(order.MaNv))
            {
                var NV = _context.NguoiDungs
                    .FirstOrDefault(nv => nv.MaNv == order.MaNv && nv.ChucVu == "Nhân viên");
                tenNhanVien = NV?.HoTen;
            }

            double originalTotal = order.ChiTietDonHangs.Sum(c => c.GiaBan * c.SoLuong) ?? 0;

            double discountAmount = Math.Max(0, originalTotal - (order.TongTien ?? 0));

            var model = new CTDHViewModel
            {
                MaDh = order.MaDh,
                ThoigianDat = order.ThoiGianDat,
                TrangThaiDh = order.TrangThaiDh,
                TongTien = order.TongTien,
                ThanhToan = order.ThanhToan,
                MaNv = order.MaNv,
                tenNv = tenNhanVien ?? "N/A",
                MaBan = order.MaBan,
                viTri = order.MaBanNavigation?.KhuVucBan ?? "N/A",
                MaKh = order.MaKh,
                tenKh = KH?.TenKh ?? "Unknown",
                SDT = KH?.Sdt ?? "N/A",
                CTDHs = order.ChiTietDonHangs?.ToList() ?? new List<ChiTietDonHang>(),
                Giam = (int)discountAmount  
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult timKhachHang(string soDienThoai)  
        {
            TempData["SearchPhone"] = soDienThoai; 

            var customer = _context.KhachHangs.FirstOrDefault(c => c.Sdt == soDienThoai);
            if (customer != null)
            {
                TempData["CustomerName"] = customer.TenKh;
                TempData["CustomerPoints"] = customer.DiemTichLuy ?? 0;  
                TempData["IsNewCustomer"] = false;
            }
            else
            {
                TempData["CustomerName"] = "";  
                TempData["CustomerPoints"] = 0;  
                TempData["IsNewCustomer"] = true;
            }

            TempData.Keep("SelectedCategory");
            TempData.Keep("SearchString");

            return RedirectToAction("SanPham", new
            {
                loaiSanPham = TempData["SelectedCategory"]?.ToString(),
                search = TempData["SearchString"]?.ToString()
            });
        }

        [HttpGet]
        public IActionResult staffDashboard()
        {
            var model = new DashboardStaffViewModel();
            var today = DateTime.Today;

            // Lấy thời gian bắt đầu từ session, nếu chưa có thì tạo mới
            var loginStartTime = HttpContext.Session.GetString("LoginStartTime");
            if (string.IsNullOrEmpty(loginStartTime))
            {
                var startTime = DateTime.Now;
                HttpContext.Session.SetString("LoginStartTime", startTime.ToString("yyyy-MM-dd HH:mm:ss"));
                model.loginStartTime = startTime;
            }
            else
            {
                model.loginStartTime = DateTime.ParseExact(loginStartTime, "yyyy-MM-dd HH:mm:ss", null);
            }

            model.todayOrder = _context.DonHangs.Where(o => o.ThoiGianDat.Value.Date == today)
                                .Count();
            model.orderCompleted = _context.DonHangs.Where(o => o.ThoiGianDat.Value.Date == today
                                        && o.TrangThaiDh == "Hoàn thành")
                                .Count();   
            model.proOrderCount = _context.DonHangs.Where(o => o.ThoiGianDat.Value.Date == today
                                        && o.TrangThaiDh == "Đang xử lý")
                                .Count();
            model.revenueToday = _context.DonHangs.Where(o => o.ThoiGianDat.HasValue && o.ThoiGianDat.Value.Date == today)
                                .Sum(o => (int)(o.TongTien ?? 0));
            //Lay cac don hang dang xu ly
            var orders = _context.DonHangs
                            .Where(o => o.ThoiGianDat.Value.Date == today && o.TrangThaiDh != "Hoàn thành")
                            .Include(o => o.MaKhNavigation)
                            .Include(o => o.ChiTietDonHangs)
                                .ThenInclude(ct => ct.MaMonNavigation)
                            .OrderByDescending(o => o.ThoiGianDat)
                            .Take(3)
                            .ToList();
            foreach(var order in orders)
            {
                var processOrder = new OrderInProcessing
                {
                    orderId = order.MaDh,
                    customerName = order.MaKhNavigation?.TenKh ?? "Anonymous",
                    orderTime = order.ThoiGianDat ?? DateTime.Now,
                    status = order.TrangThaiDh,
                    items = new List<orderItem>()
                };
                if (order.ChiTietDonHangs != null)
                {
                    foreach (var item in order.ChiTietDonHangs)
                    {
                        processOrder.items.Add(new orderItem
                        {
                            productName = item.MaMonNavigation.TenMon,
                            quantity = item.SoLuong ?? 0
                        });
                    }
                }
                model.processOrders.Add(processOrder);
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult ThongTinCaNhan(string id, bool editMode = false)
        {
            var user = _context.NguoiDungs.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.EditMode = editMode;
            return View(user);
        }

        [HttpPost]
        public IActionResult ThongTinCaNhan(NguoiDung user)
        {
            var fieldsToValidate = new[] { "Sdt", "DiaChi", "Email", "TenDangNhap" };
            var keysToRemove = ModelState.Keys
                .Where(k => !fieldsToValidate.Any(f => k.Contains(f)))
                .ToList();

            foreach (var key in keysToRemove)
            {
                ModelState.Remove(key);
            }

            if (!IsValidPhoneNumber(user.Sdt))
            {
                ModelState.AddModelError("Sdt", "Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại Việt Nam hợp lệ.");
            }

            if (!IsValidEmail(user.Email))
            {
                ModelState.AddModelError("Email", "Định dạng email không hợp lệ.");
            }

            if (string.IsNullOrWhiteSpace(user.DiaChi) || user.DiaChi.Length < 10)
            {
                ModelState.AddModelError("DiaChi", "Địa chỉ phải có ít nhất 10 ký tự.");
            }

            if (string.IsNullOrWhiteSpace(user.TenDangNhap) || user.TenDangNhap.Length < 3)
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập phải có ít nhất 3 ký tự.");
            }

            var existingUserWithSameUsername = _context.NguoiDungs
                .FirstOrDefault(u => u.TenDangNhap == user.TenDangNhap && u.MaNv != user.MaNv);

            if (existingUserWithSameUsername != null)
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã được sử dụng bởi người khác.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = _context.NguoiDungs.Find(user.MaNv);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    existingUser.Sdt = user.Sdt?.Trim();
                    existingUser.DiaChi = user.DiaChi?.Trim();
                    existingUser.Email = user.Email?.Trim().ToLower();
                    existingUser.TenDangNhap = user.TenDangNhap?.Trim();

                    _context.Update(existingUser);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Đã cập nhật thông tin thành công";
                    return RedirectToAction("ThongTinCaNhan", new { id = user.MaNv });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Lỗi khi cập nhật thông tin: " + ex.Message;
                    ViewBag.EditMode = true;
                }
            }
            else
            {
                ViewBag.EditMode = true;
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin đã nhập";
            }

            return View(user);
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "");

            var phoneRegex = new System.Text.RegularExpressions.Regex(@"^(\+84|84|0[3|5|7|8|9])[0-9]{8,9}$");
            return phoneRegex.IsMatch(phoneNumber);
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        [HttpPost]
        [Route("DoiMatKhau")]
        public IActionResult DoiMatKhau(string MatKhauCu, string MatKhauMoi, string XacNhanMatKhau)
        {
            try
            {
                var userId = HttpContext.Session.GetString("maNV");

                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn" });
                }

                var user = _context.NguoiDungs.Find(userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin người dùng" });
                }

                var errors = new Dictionary<string, string>();

                if (string.IsNullOrWhiteSpace(MatKhauCu))
                {
                    errors.Add("MatKhauCu", "Vui lòng nhập mật khẩu hiện tại");
                }
                else if (!VerifyPassword(MatKhauCu, user.MatKhau))
                {
                    errors.Add("MatKhauCu", "Mật khẩu hiện tại không chính xác");
                }

                if (string.IsNullOrWhiteSpace(MatKhauMoi))
                {
                    errors.Add("MatKhauMoi", "Vui lòng nhập mật khẩu mới");
                }
                else if (MatKhauMoi.Length < 6)
                {
                    errors.Add("MatKhauMoi", "Mật khẩu mới phải có ít nhất 6 ký tự");
                }
                else if (!IsStrongPassword(MatKhauMoi))
                {
                    errors.Add("MatKhauMoi", "Mật khẩu phải có ít nhất 1 chữ hoa, 1 chữ thường và 1 số");
                }

                if (string.IsNullOrWhiteSpace(XacNhanMatKhau))
                {
                    errors.Add("XacNhanMatKhau", "Vui lòng xác nhận mật khẩu mới");
                }
                else if (MatKhauMoi != XacNhanMatKhau)
                {
                    errors.Add("XacNhanMatKhau", "Mật khẩu xác nhận không khớp");
                }

                if (errors.Any())
                {
                    return Json(new { success = false, errors = errors });
                }

                user.MatKhau = HashPassword(MatKhauMoi);
                _context.Update(user);
                _context.SaveChanges();

                return Json(new { success = true, message = "Đổi mật khẩu thành công" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DoiMatKhau: {ex.Message}");
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
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
    