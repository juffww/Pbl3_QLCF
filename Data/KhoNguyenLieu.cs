using System;
using System.Collections.Generic;

namespace pbl3_QLCF.Data;

public partial class KhoNguyenLieu
{
    public string MaNl { get; set; } = null!;

    public string TenNl { get; set; } = null!;

    public DateOnly? NgayNhap { get; set; }

    public DateOnly? Hsd { get; set; }

    public DateOnly? Nsx { get; set; }

    public string? MaNhaCungCap { get; set; }

    public string? DonViTinh { get; set; }

    public int? Sl { get; set; }

    public virtual ICollection<CongThucMonAn> CongThucMonAns { get; set; } = new List<CongThucMonAn>();

    public virtual NhaCungCap? MaNhaCungCapNavigation { get; set; }
}
