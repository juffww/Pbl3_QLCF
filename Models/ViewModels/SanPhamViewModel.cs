using pbl3_QLCF.Data;
using System.Collections.Generic;

namespace pbl3_QLCF.Models
{
    public class SanPhamViewModel
    {
        public List<ThucDon> ThucDons { get; set; }
        public DonHang DonHangHienTai { get; set; }
        public List<string> ProductTypes { get; set; } = new List<string>();
        public string SearchString { get; set; }
    }
}