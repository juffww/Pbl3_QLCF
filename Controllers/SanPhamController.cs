using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pbl3_QLCF.Data;

namespace pbl3_QLCF.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly Pbl3Context _context;
        private const int PageSize = 8; // Số sản phẩm mỗi trang
        public SanPhamController(Pbl3Context context)
        {
            _context = context;
        }
        public IActionResult Index()
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
    }
}
