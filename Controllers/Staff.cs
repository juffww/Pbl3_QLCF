using Microsoft.AspNetCore.Mvc;
using pbl3_QLCF.ViewModels;
using pbl3_QLCF.Data;
using pbl3_QLCF.Models.Authentication;
namespace pbl3_QLCF.Controllers
{
    //[Authentication]
    public class Staff : Controller
    {
        private readonly Pbl3Context _context;

        public Staff(Pbl3Context context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult DonHang()
        {
            return View();
        }
        [HttpGet]
        public IActionResult staffDashboard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SanPham(string loaiSanPham = null, string search = null)
        {
            var donHang = GetOrCreateDonHang();

            // Get all product IDs in the cart
            var cartProductIds = donHang.ChiTietDonHangs.Select(c => c.MaMon).ToList();

            // Get filtered products based on category/search
            var filteredProducts = string.IsNullOrEmpty(search)
                ? GetThucDons(loaiSanPham)
                : _context.ThucDons.Where(t => (t.TenMon.Contains(search) || t.TenLoai.Contains(search)) &&
                                      (string.IsNullOrEmpty(loaiSanPham) || t.TenLoai == loaiSanPham)).ToList();

            // Get the IDs of filtered products
            var filteredProductIds = filteredProducts.Select(p => p.MaMon).ToList();

            // Get ALL products in cart
            var cartProducts = _context.ThucDons
                .Where(t => cartProductIds.Contains(t.MaMon))
                .ToList();

            var model = new SanPhamViewModel
            {
                //ThucDons = allProducts,
                ThucDons = filteredProducts,
                Cart = cartProducts,
                DonHangHienTai = donHang,
                ProductTypes = GetDistinctProductTypes(),
                SearchString = search
            };

            // Store selected category in TempData
            TempData["SelectedCategory"] = loaiSanPham;
            TempData["SearchString"] = search;

            return View(model);
        }

        [HttpPost]
        public IActionResult TimKiem(string search)
        {
            var model = new SanPhamViewModel
            {
                ThucDons = string.IsNullOrEmpty(search)
                    ? _context.ThucDons.ToList()
                    : _context.ThucDons.Where(t => t.TenMon.Contains(search) || t.TenLoai.Contains(search)).ToList(),
                DonHangHienTai = GetOrCreateDonHang()
            };

            return RedirectToAction("SanPham", new { loaiSanPham = TempData["SelectedCategory"]?.ToString(), search });
        }

        [HttpPost]
        public IActionResult ThemVaoDonHang(string maMon, int soLuong = 1, string ghiChu = "")
        {
            // Get current order from session
            var donHang = GetOrCreateDonHang(); 

            // Get product details
            var thucDon = _context.ThucDons.FirstOrDefault(m => m.MaMon == maMon);
            if (thucDon == null)
            {
                return RedirectToAction("SanPham");
            }

            // Check if item already exists in order
            var chiTiet = donHang.ChiTietDonHangs.FirstOrDefault(c => c.MaMon == maMon);
            if (chiTiet != null)
            {
                // Update existing item
                chiTiet.SoLuong += soLuong;
                if (!string.IsNullOrEmpty(ghiChu))
                {
                    chiTiet.GhiChu = ghiChu;
                }
            }
            else
            {
                // Add new item
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

            // Recalculate total
            CapNhatTongTien(donHang);

            // Save to session
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
                    // Remove the item
                    donHang.ChiTietDonHangs.Remove(chiTiet);
                }
                else
                {
                    // Update quantity
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
            // Create new empty order
            var donHangMoi = CreateNewOrder();
            HttpContext.Session.SetObjectAsJson("DonHangHienTai", donHangMoi);
            return RedirectToAction("SanPham");
        }

        [HttpPost]
        public IActionResult HoanTatDonHang(string tenKhachHang, string soDienThoai, string ghiChuDonHang)
        {
            var donHang = HttpContext.Session.GetObjectFromJson<DonHang>("DonHangHienTai");
            if (donHang == null || !donHang.ChiTietDonHangs.Any())
            {
                return RedirectToAction("SanPham");
            }

            // Find or create customer if info provided
            string? maKh = null;
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
            }

            // Get current user ID (staff ID)
            var maNv = User.Identity.IsAuthenticated ? User.FindFirst("MaNv")?.Value : null;

            // Create order in database
            var orderToSave = new DonHang
            {
                MaDh = donHang.MaDh,
                MaKh = maKh,
                MaNv = maNv,
                ThoiGianDat = DateTime.Now,
                TongTien = donHang.TongTien,
                ThanhToan = "Chưa thanh toán",
                TrangThaiDh = "Mới",
                // Add note if provided
                // Note: You might need to add a GhiChu field to your DonHang model
            };

            _context.DonHangs.Add(orderToSave);
            _context.SaveChanges();

            // Save order details
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

            // Create new empty order
            var donHangMoi = CreateNewOrder();
            HttpContext.Session.SetObjectAsJson("DonHangHienTai", donHangMoi);

            TempData["SuccessMessage"] = "Đơn hàng đã được tạo thành công!";
            return RedirectToAction("SanPham");
        }

        // Helper methods
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

            // Always ensure ChiTietDonHangs is initialized
            if (donHang.ChiTietDonHangs == null)
            {
                donHang.ChiTietDonHangs = new List<ChiTietDonHang>();
            }

            // Save back to session to ensure consistency
            HttpContext.Session.SetObjectAsJson("DonHangHienTai", donHang);
            return donHang;
        }

        private DonHang CreateNewOrder()
        {
            //var tenNv = HttpContext.Session.GetString("TenDangNhap");
            //var MaNV = _context.NguoiDungs.FirstOrDefault(nv => nv.TenDangNhap == tenNv);
            return new DonHang
            {
                MaDh = GenerateNewOrderId(),
                ThoiGianDat = DateTime.Now,
                TongTien = 0,
                ThanhToan = "Chưa thanh toán",
                TrangThaiDh = "Mới",
                ChiTietDonHangs = new List<ChiTietDonHang>(),
                //MaNv = MaNV.MaNv
            };
        }

        private string GenerateNewOrderId()
        {
            // Format: DH + sequential number (DH001, DH002, DH003, etc.)
            string prefix = "DH";

            // Get the latest order with this prefix
            var latestOrder = _context.DonHangs
                .Where(o => o.MaDh.StartsWith(prefix))
                .OrderByDescending(o => o.MaDh)
                .FirstOrDefault();

            if (latestOrder == null)
            {
                // First order ever
                return prefix + "001";
            }
            else
            {
                try
                {
                    // Extract the numeric part
                    string currentNumber = latestOrder.MaDh.Substring(prefix.Length);

                    // Use TryParse to safely convert the string to an integer
                    if (int.TryParse(currentNumber, out int currentVal))
                    {
                        int nextNumber = currentVal + 1;
                        return prefix + nextNumber.ToString("D3"); // Format as 3 digits with leading zeros
                    }
                    else
                    {
                        // If parsing fails, start from 1
                        return prefix + "001";
                    }
                }
                catch
                {
                    // Handle any unexpected errors by providing a fallback
                    return prefix + "001";
                }
            }
        }

        private void CapNhatTongTien(DonHang donHang)
        {
            double tongTien = 0;
            foreach (var item in donHang.ChiTietDonHangs)
            {
                tongTien += (double)(item.GiaBan * item.SoLuong);
            }
            donHang.TongTien = tongTien;
        }

        // Helper to get distinct product types
        public List<string> GetDistinctProductTypes()
        {
            return _context.ThucDons
                .Select(t => t.TenLoai)
                .Distinct()
                .Where(t => t != null)
                .ToList();
        }
    }
}
