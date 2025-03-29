using System;
using System.Collections.Generic;

namespace pbl3_QLCF.Data;

public partial class ChiTietDonHang
{
    public string MaDh { get; set; } = null!;

    public string MaMon { get; set; } = null!;

    public int? SoLuong { get; set; }

    public double? GiaBan { get; set; }

    public string? GhiChu { get; set; }

    public virtual DonHang MaDhNavigation { get; set; } = null!;

    public virtual ThucDon MaMonNavigation { get; set; } = null!;
}
