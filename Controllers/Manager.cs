using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using pbl3_QLCF.Data;
using pbl3_QLCF.Models.Authentication;

namespace pbl3_QLCF.Controllers
{
    //[Authentication]
    public class Manager : Controller
    {
        private readonly Pbl3Context _context;
        private const int PageSize = 8;

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
        [Route("ThemSanPham")]
        [HttpGet]
        public IActionResult ThemSanPham()
        {
            return View();
        }

        
        [Route("ThemSanPham")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //ASP.NET Core yêu cầu một mã bảo mật trong mỗi request POST.
        public IActionResult ThemSanPham(ThucDon product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra mã sản phẩm đã tồn tại chưa
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
        // Xử lý hiển thị form chỉnh sửa sản phẩm
        public IActionResult EditProduct(string id)
        {
            var product = _context.ThucDons.FirstOrDefault(p => p.MaMon == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Xử lý lưu thông tin chỉnh sửa
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
                    //Nếu có lỗi trong quá trình cập nhật (ví dụ: lỗi CSDL) → Bắt lỗi và thêm thông báo
                    //lỗi vào ModelState.
                }
            }
            return View(product);
        }
        // Xử lý xóa sản phẩm
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

                // Kiểm tra xem sản phẩm có đang được sử dụng trong chi tiết đơn hàng không
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
