using System;
using System.Collections.Generic;

namespace pbl3_QLCF.Data;

public partial class KhachHang
{
    public string MaKh { get; set; } = null!;

    public string TenKh { get; set; } = null!;

    public string? Sdt { get; set; }

    public int? DiemTichLuy { get; set; }
    public string? LoaiKH {  get; set; }
    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
