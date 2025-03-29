using System;
using System.Collections.Generic;

namespace pbl3_QLCF.Data;

public partial class HoaDon
{
    public string MaHd { get; set; } = null!;

    public string? MaDh { get; set; }

    public double? TongTien { get; set; }

    public DateOnly? Ngay { get; set; }

    public bool? TrangThai { get; set; }

    public virtual DonHang? MaDhNavigation { get; set; }
}
