using pbl3_QLCF.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pbl3_QLCF.ViewModels
{
    public class SanPhamViewModel
    {
        public List<ThucDon> ThucDons { get; set; }
        public List<ThucDon> Cart { get; set; }
        [Required]
        public DonHang DonHangHienTai { get; set; }
        public List<string> ProductTypes { get; set; } = new List<string>();
        public string SearchString { get; set; }
        public int DTL { get; set; }
    }
}