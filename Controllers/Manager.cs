using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using pbl3_QLCF.Data;
using pbl3_QLCF.Models.Authentication;

namespace pbl3_QLCF.Controllers
{
    //[Authentication]
    public class Manager : Controller
    {
        private readonly Pbl3Context _context;
        private const int PageSize = 8; // Số sản phẩm mỗi trang

        public Manager(Pbl3Context context)
        {
            _context = context;
        }

        public IActionResult magDashboard()
        {
            return View();
        }

        public IActionResult SanPham(int page = 1, string category = "all", string search = "")
        {
            // Lấy danh sách sản phẩm từ CSDL
            IQueryable<ThucDon> query = _context.ThucDons;

            // Lọc theo danh mục
            if (category != "all")
            {
                query = query.Where(p => p.TenLoai == category);
            }

            // Tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.TenMon.Contains(search));
            }

            // Tính toán phân trang
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            // Lấy dữ liệu cho trang hiện tại
            var products = query
                .OrderBy(p => p.MaMon)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Truyền dữ liệu sang View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentCategory = category;
            ViewBag.SearchString = search;

            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProduct([FromBody] ThucDon product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra mã sản phẩm đã tồn tại chưa
                    var existingProduct = _context.ThucDons.FirstOrDefault(p => p.MaMon == product.MaMon);
                    if (existingProduct != null)
                    {
                        return Json(new { success = false, message = "Mã sản phẩm đã tồn tại" });
                    }
                    // Thêm sản phẩm mới
                    _context.ThucDons.Add(product);
                    _context.SaveChanges();
                    return Json(new { success = true, message = "Thêm sản phẩm thành công" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Lỗi khi thêm sản phẩm: " + ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            }
        }
        public IActionResult DonHang()
        {
            return View();
        }

        public IActionResult KhachHang()
        {
            return View();
        }
    }
}
