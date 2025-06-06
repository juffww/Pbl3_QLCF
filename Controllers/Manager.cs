using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using pbl3_QLCF.Data;
using pbl3_QLCF.Models.Authentication;
using pbl3_QLCF.Service;
using pbl3_QLCF.ViewModels;

namespace pbl3_QLCF.Controllers
{
    //[Authentication]
    public class Manager : Controller
    {
        private readonly Pbl3Context _context;
        private const int PageSize = 8;
        private const int PageOrderSize = 5;
        public Manager(Pbl3Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult SanPham(int page = 1, string category = "all", string search = "")
        {
            IQueryable<ThucDon> query = _context.ThucDons;

            if (category != "all")
            {
                query = query.Where(p => p.TenLoai == category);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.TenMon.Contains(search));
            }

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            var products = query
                .OrderBy(p => p.MaMon)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentCategory = category;
            ViewBag.SearchString = search;

            return View(products);
        }
        [Route("ThemSanPham")]
        [HttpGet]
        public IActionResult ThemSanPham()
        {
            return View();
        }


        [Route("ThemSanPham")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemSanPham(ThucDon product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = _context.ThucDons.FirstOrDefault(p => p.MaMon == product.MaMon);
                    if (existingProduct != null)
                    {
                        TempData["ErrorMessage"] = "Lỗi: Mã sản phẩm đã tồn tại";
                        return View(product);
                    }
                    _context.ThucDons.Add(product);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Thêm sản phẩm thành công";
                    return RedirectToAction("SanPham");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Lỗi khi thêm sản phẩm: " + ex.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin sản phẩm";
            }
            return View(product);
        }
        [HttpGet]
        public IActionResult EditProduct(string id)
        {
            var product = _context.ThucDons.FirstOrDefault(p => p.MaMon == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(ThucDon product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công";
                    return RedirectToAction("SanPham");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật: " + ex.Message);
                }
            }
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProduct(string id)
        {
            try
            {
                var product = _context.ThucDons.Find(id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm";
                    return RedirectToAction("SanPham");
                }

                bool isInUse = _context.ChiTietDonHangs.Any(c => c.MaMon == id);
                if (isInUse)
                {
                    TempData["ErrorMessage"] = "Không thể xóa sản phẩm này vì đã có trong đơn hàng";
                    return RedirectToAction("SanPham");
                }

                _context.ThucDons.Remove(product);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Xóa sản phẩm thành công";
                return RedirectToAction("SanPham");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa sản phẩm: " + ex.Message;
                return RedirectToAction("SanPham");
            }
        }

        //-----------------------------Đơn Hàng----------------------------------------------------
        [HttpGet]
        public IActionResult DonHang(int page = 1, string category = "all", string search = "")
        {
            IQueryable<DonHang> query = _context.DonHangs;
            if (category != "all")
            {
                switch (category)
                {
                    case "new":
                        query = query.Where(dh => dh.TrangThaiDh == "Mới");
                        break;
                    case "processing":
                        query = query.Where(dh => dh.TrangThaiDh == "Đang xử lý");
                        break;
                    case "completed":
                        query = query.Where(dh => dh.TrangThaiDh == "Hoàn thành");
                        break;
                }
            }
            //search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(dh =>
                    dh.MaDh.Contains(search) ||
                    dh.MaKh.Contains(search) ||
                    dh.MaNv.Contains(search) ||
                    dh.MaBan.Contains(search)
                );
            }
            query = query.OrderByDescending(dh => dh.ThoiGianDat);

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageOrderSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentCategory = category;
            ViewBag.SearchString = search;

            var orders = query
                .Skip((page - 1) * PageOrderSize)
                .Take(PageOrderSize)
                .ToList();

            return View(orders);
        }
        [HttpGet]
        public IActionResult XemDonHang(string id)
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
            var KH = _context.KhachHangs.FirstOrDefault(kh => kh.MaKh == order.MaKh);
            string? tenNhanVien = null;
            if (!string.IsNullOrEmpty(order.MaNv))
            {
                var NV = _context.NguoiDungs
                    .Where(nv => nv.ChucVu == "Nhân viên")
                    .FirstOrDefault(nv => nv.MaNv == order.MaNv);
                tenNhanVien = NV?.HoTen;
            }
            int originalTotal = order.ChiTietDonHangs.Sum(c => c.GiaBan * c.SoLuong) ?? 0;

            int discountAmount = Math.Max(0, originalTotal - (order.TongTien ?? 0));

            var model = new CTDHViewModel
            {
                MaDh = order.MaDh,
                ThoigianDat = order.ThoiGianDat,
                TrangThaiDh = order.TrangThaiDh,
                TongTien = order.TongTien,
                ThanhToan = order.ThanhToan,
                MaNv = order.MaNv,
                tenNv = tenNhanVien ?? "N/A",
                MaBan = order.MaBan ?? "N/A",
                viTri = order.MaBanNavigation?.KhuVucBan ?? "N/A",
                MaKh = order.MaKh,
                tenKh = KH?.TenKh ?? "Unknown",
                SDT = KH?.Sdt ?? "N/A",
                CTDHs = order.ChiTietDonHangs?.ToList() ?? new List<ChiTietDonHang>(),
                Giam = discountAmount
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XoaDonHang(string id)
        {
            try
            {
                var order = _context.DonHangs.Find(id);
                if (order == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm để xóa";
                    return RedirectToAction("DonHang");
                }
                var orderDetails = _context.ChiTietDonHangs.Where(od => od.MaDh == id).ToList();
                _context.ChiTietDonHangs.RemoveRange(orderDetails);
                _context.DonHangs.Remove(order);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Đã xóa đơn hàng thành công";
                return RedirectToAction("DonHang");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa sản phẩm" + e.Message;
                return RedirectToAction("DonHang");
            }
        }
        //---------------------------------Khách hàng-----------------------
        [HttpGet]
        public IActionResult KhachHang(int page = 1, string category = "all", string search = "")
        {
            CustomerService customerService = new CustomerService(_context);
            customerService.UpdateCustomerTypes();
            IQueryable<KhachHang> query = _context.KhachHangs;
            if (category != "all")
            {
                query = query.Where(q => q.LoaiKH == category);
            }
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(q => q.TenKh.Contains(search));
            }
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            var KHs = query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentCategory = category;
            ViewBag.SearchString = search;
            return View(KHs);
        }
        [HttpGet]
        public IActionResult lichSuMua(string id, int page = 1, string category = "all", string search = "")
        {
            var customer = _context.KhachHangs.FirstOrDefault(c => c.MaKh == id);
            if (customer == null)
            {
                return NotFound();
            }
            var order = _context.DonHangs.Where(o => o.MaKh == id).ToList();
            var model = new CTKHViewModel
            {
                CustomerId = id,
                CustomerName = customer.TenKh,
                CustomerType = customer.LoaiKH,
                SDT = customer.Sdt,
                LoyaltyPoints = customer.DiemTichLuy,
                TotalSpent = order.Sum(o => o.TongTien ?? 0),
                OrderCount = order.Count(),
                AverageOrderValue = order.Any() ? order.Average(o => o.TongTien ?? 0) : 0,
                LastPurchaseDate = order.Any() ? order.Max(o => o.ThoiGianDat) : null,
                Orders = order.Select(o => new CTKHViewModel.OrderSummary
                {
                    OrderId = o.MaDh,
                    OrderTime = (DateTime)o.ThoiGianDat,
                    TotalAmount = o.TongTien ?? 0,
                }).ToList()
            };

            ViewBag.CurrentPage = page;
            ViewBag.CurrentCategory = category;
            ViewBag.SearchString = search;

            return View(model);
        }
        //--------------------------------Dashboard----------------------------
        [HttpGet]
        public IActionResult magDashboard()
        {
            var model = new DashboardMagViewModel();
            var today = DateTime.Today;
            var customerToday = DateTime.Now;
            var yesterday = DateTime.Today.AddDays(-1);
            var thisWeek = DateTime.Today.AddDays(-7);

            // Doanh thu hôm nay
            model.todayRevenue = _context.DonHangs.Where(o => o.ThoiGianDat.HasValue && o.ThoiGianDat.Value.Date == today)
                                    .Sum(o => (int)(o.TongTien ?? 0));
            var yesterdayRevenue = _context.DonHangs.Where(o => o.ThoiGianDat.HasValue && o.ThoiGianDat.Value.Date == yesterday)
                                    .Sum(o => (int)(o.TongTien ?? 0));
            model.todayPercent = 0;
            if (yesterdayRevenue > 0)
            {
                model.todayPercent = (double)(model.todayRevenue - yesterdayRevenue) / yesterdayRevenue * 100;
                model.todayPercent = Math.Round(model.todayPercent, 2);
            }
            else if (model.todayRevenue > 0)
            {
                model.todayPercent = 100;
            }

            // Đơn hàng hôm nay
            model.todayOrder = _context.DonHangs
                .Where(o => o.ThoiGianDat.HasValue && o.ThoiGianDat.Value.Date == today)
                .Count();

            var yesterdayOrder = _context.DonHangs
                .Where(o => o.ThoiGianDat.HasValue && o.ThoiGianDat.Value.Date == yesterday)
                .Count();

            model.orderPercent = 0;
            if (yesterdayOrder > 0)
            {
                model.orderPercent = (double)(model.todayOrder - yesterdayOrder) / yesterdayOrder * 100;
                model.orderPercent = Math.Round(model.orderPercent, 2);
            }
            else if (model.todayOrder > 0)
            {
                model.orderPercent = 100;
            }

            // Khách hàng mới
            var allCustomer = _context.KhachHangs.ToList();
            var newCustomerThisWeek = new List<KhachHang>();
            foreach (var customer in allCustomer)
            {
                var firstOrder = _context.DonHangs
                                .Where(o => o.MaKh == customer.MaKh)
                                .OrderBy(o => o.ThoiGianDat)
                                .FirstOrDefault();
                if (firstOrder != null && firstOrder.ThoiGianDat.HasValue &&
                    firstOrder.ThoiGianDat.Value >= thisWeek && firstOrder.ThoiGianDat.Value <= customerToday)
                {
                    newCustomerThisWeek.Add(customer);
                }
            }
            model.newCustomerCount = newCustomerThisWeek.Count();

            var lastWeek = thisWeek.AddDays(-7);
            var newCustomerLastWeek = 0;
            foreach (var customer in allCustomer)
            {
                var firstOrder = _context.DonHangs
                                .Where(d => d.MaKh == customer.MaKh)
                                .OrderBy(d => d.ThoiGianDat)
                                .FirstOrDefault();
                if (firstOrder != null && firstOrder.ThoiGianDat.HasValue &&
                    firstOrder.ThoiGianDat.Value >= lastWeek && firstOrder.ThoiGianDat.Value < thisWeek)
                {
                    newCustomerLastWeek++;
                }
            }
            model.customerPercent = 0;
            if (newCustomerLastWeek > 0)
            {
                model.customerPercent = (double)(model.newCustomerCount - newCustomerLastWeek) / newCustomerLastWeek * 100;
                model.customerPercent = Math.Round(model.customerPercent, 2);
            }
            else if (model.newCustomerCount > 0)
            {
                model.customerPercent = 100;
            }

            // Sản phẩm bán chạy
            var month = DateTime.Now.AddDays(-30);
            model.topSellingProduct = _context.ChiTietDonHangs
                .Where(ct => ct.MaDhNavigation.ThoiGianDat.HasValue && ct.MaDhNavigation.ThoiGianDat.Value.Date >= month)
                .GroupBy(ct => new { ct.MaMonNavigation.TenMon })
                .Select(g => new topSelling()
                {
                    productName = g.Key.TenMon,
                    quantity = g.Sum(ct => ct.SoLuong.HasValue ? ct.SoLuong.Value : 0)
                })
                .OrderByDescending(x => x.quantity)
                .Take(5)
                .ToList();

            // Lấy đơn hàng gần nhất
            model.recentOrders = _context.DonHangs
                                 .OrderByDescending(o => o.ThoiGianDat)
                                 .Take(10)
                                 .Select(o => new recentOrder()
                                 {
                                     orderID = o.MaDh,
                                     customerName = o.MaKhNavigation.TenKh,
                                     tongTien = o.TongTien ?? 0,
                                     status = o.TrangThaiDh
                                 })
                                 .ToList();

            model.revenueTrend = new List<revenueTrendItem>();
            DateTime startDate = DateTime.Today.AddDays(-8); // Lấy 9 ngày (từ -8 đến 0)

            for (int i = 0; i < 9; i++)
            {
                DateTime date = startDate.AddDays(i);
                string dayName = GetVietnameseDayName(date.DayOfWeek);

                int revenue = _context.DonHangs
                    .Where(o => o.ThoiGianDat.HasValue && o.ThoiGianDat.Value.Date == date.Date)
                    .Sum(o => o.TongTien ?? 0);

                model.revenueTrend.Add(new revenueTrendItem()
                {
                    time = dayName,
                    revenue = revenue,
                    date = date.ToString("dd/MM") // Thêm trường date để hiển thị
                });
            }

            return View(model);
        }

        private string GetVietnameseDayName(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday: return "T2";
                case DayOfWeek.Tuesday: return "T3";
                case DayOfWeek.Wednesday: return "T4";
                case DayOfWeek.Thursday: return "T5";
                case DayOfWeek.Friday: return "T6";
                case DayOfWeek.Saturday: return "T7";
                case DayOfWeek.Sunday: return "CN";
                default: return "";
            }
        }
        [HttpGet]
        public IActionResult GetMonthlyRevenue(int year)
        {
            try
            {
                var monthlyData = new List<object>();

                for (int month = 1; month <= 12; month++)
                {
                    var startDate = new DateTime(year, month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);

                    var monthRevenue = _context.DonHangs
                        .Where(o => o.ThoiGianDat.HasValue &&
                                   o.ThoiGianDat.Value.Date >= startDate.Date &&
                                   o.ThoiGianDat.Value.Date <= endDate.Date)
                        .Sum(o => o.TongTien ?? 0);

                    monthlyData.Add(new
                    {
                        month = month,
                        revenue = monthRevenue,
                        name = $"Tháng {month}"
                    });
                }

                return Json(new { success = true, data = monthlyData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        //-------------------NhanVien-------------------
        [HttpGet]
        public IActionResult NhanVien(int page = 1, string search = "")
        {
            IQueryable<NguoiDung> query = _context.NguoiDungs/*.Where(n => n.ChucVu == "Nhân viên")*/;
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(q => q.HoTen.Contains(search) || q.Sdt.Contains(search));
            }
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            var NVs = query.Skip((page - 1) * PageSize)
                        .Take(PageSize)
                        .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchString = search;
            return View(NVs);
        }
        [HttpGet]
        public IActionResult ThemNhanVien()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ThemNhanVien(NguoiDung user)
        {
            if (ModelState.IsValid)
            {
                var existingNV = _context.NguoiDungs.FirstOrDefault(n => n.MaNv == user.MaNv);
                if (existingNV != null)
                {
                    TempData["ErrorMessage"] = "Lỗi: Mã nhân viên đã tồn tại";
                    return View(user);
                }
                _context.NguoiDungs.Add(user);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Thêm người dùng thành công";
                return RedirectToAction("NhanVien");
            }
            else
            {
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin nhân viên";
            }
            return View(user);
        }
        [HttpGet]
        public IActionResult xemUser(string id)
        {
            if (id == null)
            {
                id = HttpContext.Session.GetString("maNV");
            }

            var user = _context.NguoiDungs.FirstOrDefault(u => u.MaNv == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public IActionResult DeleteNhanVien(string id)
        {
            try
            {
                var product = _context.ThucDons.Find(id);
                var user = _context.NguoiDungs.Find(id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy người dùng cần xóa";
                    return RedirectToAction("NhanVien");
                }
                _context.NguoiDungs.Remove(user);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Xóa người dùng thành công";
                return RedirectToAction("NhanVien");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa người dùng: " + ex.Message;
                return RedirectToAction("NhanVien");
            }
        }
        [HttpPost]
        public IActionResult UpdateNhanVien(string MaNv, string ChucVu, string CaLamViec)
        {
            try
            {
                var user = _context.NguoiDungs.FirstOrDefault(u => u.MaNv == MaNv);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy nhân viên cần cập nhật";
                    return RedirectToAction("NhanVien");
                }

                if (string.IsNullOrEmpty(ChucVu) || string.IsNullOrEmpty(CaLamViec))
                {
                    TempData["ErrorMessage"] = "Vui lòng chọn đầy đủ thông tin chức vụ và ca làm việc";
                    return RedirectToAction("xemUser", new { id = MaNv });
                }

                if (ChucVu != "Quản lý" && ChucVu != "Nhân viên")
                {
                    TempData["ErrorMessage"] = "Chức vụ không hợp lệ";
                    return RedirectToAction("xemUser", new { id = MaNv });
                }

                if (CaLamViec != "Ca sáng" && CaLamViec != "Ca chiều" && CaLamViec != "Ca tối")
                {
                    TempData["ErrorMessage"] = "Ca làm việc không hợp lệ";
                    return RedirectToAction("xemUser", new { id = MaNv });
                }

                user.ChucVu = ChucVu;
                user.CaLamViec = CaLamViec;

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật thông tin nhân viên thành công";

                return RedirectToAction("xemUser", new { id = MaNv });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi cập nhật thông tin: " + ex.Message;
                return RedirectToAction("xemUser", new { id = MaNv });
            }
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
        //[Route("DoiMatKhau")]
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

                // Dictionary để chứa các lỗi
                var errors = new Dictionary<string, string>();

                // Kiểm tra mật khẩu cũ
                if (string.IsNullOrWhiteSpace(MatKhauCu))
                {
                    errors.Add("MatKhauCu", "Vui lòng nhập mật khẩu hiện tại");
                }
                else if (!VerifyPassword(MatKhauCu, user.MatKhau))
                {
                    errors.Add("MatKhauCu", "Mật khẩu hiện tại không chính xác");
                }

                // Kiểm tra mật khẩu mới
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

                // Kiểm tra xác nhận mật khẩu
                if (string.IsNullOrWhiteSpace(XacNhanMatKhau))
                {
                    errors.Add("XacNhanMatKhau", "Vui lòng xác nhận mật khẩu mới");
                }
                else if (MatKhauMoi != XacNhanMatKhau)
                {
                    errors.Add("XacNhanMatKhau", "Mật khẩu xác nhận không khớp");
                }

                // Nếu có lỗi, trả về danh sách lỗi
                if (errors.Any())
                {
                    return Json(new { success = false, errors = errors });
                }

                // Cập nhật mật khẩu mới
                user.MatKhau = HashPassword(MatKhauMoi);
                _context.Update(user);
                _context.SaveChanges();

                return Json(new { success = true, message = "Đổi mật khẩu thành công" });
            }
            catch (Exception ex)
            {
                // Log lỗi để debug
                Console.WriteLine($"Error in DoiMatKhau: {ex.Message}");
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            // Thêm logging để debug
            Console.WriteLine($"Verifying password. Input: '{password}', Stored: '{hashedPassword}'");

            // Nếu bạn đang dùng plain text (chỉ cho development)
            bool isMatch = password == hashedPassword;
            Console.WriteLine($"Password match result: {isMatch}");

            return isMatch;

            // Nếu bạn dùng BCrypt (recommended for production):
            // return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private string HashPassword(string password)
        {
            // Cho development, có thể dùng plain text
            return password;

            // Cho production, nên dùng BCrypt:
            // return BCrypt.Net.BCrypt.HashPassword(password);
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
