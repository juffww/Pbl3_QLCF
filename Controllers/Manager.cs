using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using pbl3_QLCF.Data;
using pbl3_QLCF.Models.Authentication;
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
            // Use Include to explicitly load the navigation properties
            var order = _context.DonHangs
                .Include(o => o.ChiTietDonHangs)
                .ThenInclude(c => c.MaMonNavigation)
                .FirstOrDefault(o => o.MaDh == id);

            if (order == null)
            {
                return NotFound();
            }
            var KH = _context.KhachHangs.FirstOrDefault(kh => kh.MaKh == order.MaKh);
            // Xử lý thông tin nhân viên an toàn
            string? tenNhanVien = null;
            if (!string.IsNullOrEmpty(order.MaNv))
            {
                var NV = _context.NguoiDungs
                    .Where(nv => nv.ChucVu == "Nhân viên")
                    .FirstOrDefault(nv => nv.MaNv == order.MaNv);
                tenNhanVien = NV?.HoTen;
            }
            // Calculate the original total based on order items
            int originalTotal = order.ChiTietDonHangs.Sum(c => c.GiaBan * c.SoLuong) ?? 0;

            // Calculate discount - the difference between original total and final total
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
                MaBan = order.MaBan,
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
        //--------------------------------Dashboard-----------------------
        [HttpGet]
        public IActionResult magDashboard()
        {
            var model = new DashboardMagViewModel();
            var today = DateTime.Today;
            var yesterday = DateTime.Now.AddDays(-1);
            var thisWeek = DateTime.Now.AddDays(-7);

            //Doanh thu hom nay
            model.todayRevenue = _context.DonHangs.Where(o => o.ThoiGianDat.Value.Date == today)
                                    .Sum(o => (int)o.TongTien);
            var yesterdayRevenue = _context.DonHangs.Where(o => o.ThoiGianDat.Value.Date == yesterday)
                                    .Sum(o => (int)o.TongTien);
            if (yesterdayRevenue > 0)
            {
                model.todayPercent = (double)(model.todayRevenue - yesterdayRevenue) / yesterdayRevenue * 100;
                model.todayPercent = Math.Round(model.todayPercent, 2);
            }
            //Don hang hom nay
            model.todayOrder = _context.DonHangs.Where(o => o.ThoiGianDat.Value.Date == today)
                                    .Count();
            var yesterdayOrder = _context.DonHangs.Where(o => o.ThoiGianDat.Value.Date == yesterday)
                                    .Count();
            if (yesterdayOrder > 0)
            {
                model.orderPercent = (double)(model.todayOrder - yesterdayOrder) / yesterdayOrder * 100;
                model.orderPercent = Math.Round(model.orderPercent, 2);
            }
            //KH moi
            var allCustomer = _context.KhachHangs.ToList();
            var newCustomerThisWeek = new List<KhachHang>();
            foreach (var customer in allCustomer)
            {
                var firstOrder = _context.DonHangs
                                .Where(o => o.MaKh == customer.MaKh)
                                .OrderByDescending(o => o.ThoiGianDat)
                                .FirstOrDefault();
                if (firstOrder != null && firstOrder.ThoiGianDat >= thisWeek && firstOrder.ThoiGianDat <= today)
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

                if (firstOrder != null && firstOrder.ThoiGianDat >= lastWeek && firstOrder.ThoiGianDat < thisWeek)
                {
                    newCustomerLastWeek++;
                }
            }
            if (newCustomerLastWeek > 0)
            {
                model.customerPercent = (double)(model.newCustomerCount - newCustomerLastWeek) / newCustomerLastWeek * 100;
                model.customerPercent = Math.Round(model.customerPercent, 2);
            }
            //San pham ban chay
            var month = DateTime.Now.AddDays(-30);
            model.topSellingProduct = _context.ChiTietDonHangs.Where(ct => ct.MaDhNavigation.ThoiGianDat.Value.Date >= month)
                                        .GroupBy(ct => new { ct.MaMonNavigation.TenMon })
                                        .Select(g => new topSelling()
                                        {
                                            productName = g.Key.TenMon,
                                            quantity = g.Sum(ct => ct.SoLuong.HasValue ? ct.SoLuong.Value : 0)
                                        })
                                        .OrderByDescending(x => x.quantity)
                                        .Take(5)
                                        .ToList();
            //Lay don hang gan nhat
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
            //Xu huong doanh thu
            model.revenueTrend = new List<revenueTrendItem>();
            DateTime startDate = DateTime.Today.AddDays(-9);
            for (int i = 0; i < 9; i++)
            {
                DateTime date = startDate.AddDays(i);
                string dayName = date.DayOfWeek.ToString();
                switch (dayName)
                {
                    case "Monday": dayName = "T2"; break;
                    case "Tuesday": dayName = "T3"; break;
                    case "Wednesday": dayName = "T4"; break;
                    case "Thursday": dayName = "T5"; break;
                    case "Friday": dayName = "T6"; break;
                    case "Saturday": dayName = "T7"; break;
                    case "Sunday": dayName = "CN"; break;
                }
                int revenue = _context.DonHangs.Where(o => o.ThoiGianDat == date)
                                .Sum(o => o.TongTien ?? 0);
                model.revenueTrend.Add(new revenueTrendItem()
                {
                    time = dayName,
                    revenue = revenue
                });
            }
            return View(model);
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
    }    
}
