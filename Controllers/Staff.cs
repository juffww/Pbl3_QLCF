using Microsoft.AspNetCore.Mvc;
using pbl3_QLCF.ViewModels;
using pbl3_QLCF.Data;
using pbl3_QLCF.Models.Authentication;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult staffDashboard()
        {
            return View();
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
        public IActionResult HoanTatDonHang(string tenKhachHang, string soDienThoai, string ghiChuDonHang, string ban = null, bool usePoints = false)
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
                // If using points, calculate how many points can actually be used
                if (usePoints && khachHang.DiemTichLuy > 0)
                {
                    double originalTotal = donHang.TongTien ?? 0;
                    int availablePoints = khachHang.DiemTichLuy ?? 0;

                    int maxUsablePoints = (int)Math.Ceiling(originalTotal / 1000);
                    int pointsToDeduct = Math.Min(availablePoints, maxUsablePoints);
                    // Calculate discount amount
                    double discountAmount = pointsToDeduct * 1000;

                    // Apply discount
                    donHang.TongTien -= discountAmount;
                    if (donHang.TongTien < 0) donHang.TongTien = 0;
                    
                    // Update customer points
                    khachHang.DiemTichLuy -= pointsToDeduct;
                }

                // Calculate new points based on the final amount (after discount)
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
            double tongTien = 0;
            foreach (var item in donHang.ChiTietDonHangs)
            {
                tongTien += (double)(item.GiaBan * item.SoLuong);
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
                CTDHs = order.ChiTietDonHangs?.ToList() ?? new List<ChiTietDonHang>()
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
    }
}
