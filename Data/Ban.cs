using System;
using System.Collections.Generic;

namespace pbl3_QLCF.Data;

public partial class Ban
{
    public string MaBan { get; set; } = null!;

    public string? TrangThai { get; set; }

    public string? KhuVucBan { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
