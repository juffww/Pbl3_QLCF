using System;
using System.Collections.Generic;

namespace pbl3_QLCF.Data;

public partial class ThucDon
{
    public string MaMon { get; set; } = null!;
    public string TenMon { get; set; } = null!;
    public string? TenLoai { get; set; }
    public string? HinhAnh { get; set; } 
    public int GiaSp { get; set; }
    public bool? TinhTrang { get; set; } 

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    public virtual ICollection<CongThucMonAn> CongThucMonAns { get; set; } = new List<CongThucMonAn>();
    public virtual ICollection<KhuyenMai> MaKms { get; set; } = new List<KhuyenMai>();
}
