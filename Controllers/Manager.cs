﻿using Microsoft.AspNetCore.Authorization.Infrastructure;
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

        public IActionResult magDashboard()
        {
            return View();
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
            if(category != "all")
            {
                switch(category)
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
            double originalTotal = order.ChiTietDonHangs.Sum(c => c.GiaBan * c.SoLuong) ?? 0;

            // Calculate discount - the difference between original total and final total
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
                MaKh = order.MaKh,
                tenKh = KH?.TenKh ?? "Unknown", 
                SDT = KH?.Sdt ?? "N/A",
                CTDHs = order.ChiTietDonHangs?.ToList() ?? new List<ChiTietDonHang>(),
                Giam = (int)discountAmount
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
                if(order == null)
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
            catch(Exception e)
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
            if(!string.IsNullOrEmpty(search))
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
        [HttpGet]
        public IActionResult CTLSMH()
        {
            return View();
        }
        
        public string customerType(string id)
        {
            var order = _context.DonHangs.Where(o => o.MaKh == id)
                                .OrderByDescending(o => o.ThoiGianDat)
                                .ToList();
            if(!order.Any())
            {
                return "Mới";
            }
            DateTime? lastPurchaseDate = order.FirstOrDefault()?.ThoiGianDat;
            if(!lastPurchaseDate.HasValue)
            {
                return "Không hoạt động";
            }
            DateTime currentDay = DateTime.Now;
            if((currentDay - lastPurchaseDate.Value).TotalDays > 90)
            {
                return "Không hoạt động";
            }
            if((currentDay - lastPurchaseDate.Value).TotalDays <= 7)
            {
                return "VIP";
            }
            if ((currentDay - lastPurchaseDate.Value).TotalDays <= 30)
            {
                return "Thường xuyên";
            }
            DateTime? firstOrderDate = order.LastOrDefault()?.ThoiGianDat;
            if (firstOrderDate.HasValue && (DateTime.Now - firstOrderDate.Value).TotalDays <= 90)
            {
                return "Mới";
            }
            return "Thường xuyên";
        }
    }
}
