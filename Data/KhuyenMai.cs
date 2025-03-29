using System;
using System.Collections.Generic;

namespace pbl3_QLCF.Data;

public partial class KhuyenMai
{
    public string MaKm { get; set; } = null!;

    public string TenKm { get; set; } = null!;

    public string? MoTa { get; set; }

    public double? GiaTriGiam { get; set; }

    public DateOnly? NgayBd { get; set; }

    public DateOnly? NgayKt { get; set; }

    public string? DkapDung { get; set; }

    public virtual ICollection<ThucDon> MaMons { get; set; } = new List<ThucDon>();
}
