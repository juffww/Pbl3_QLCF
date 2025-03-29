using System;
using System.Collections.Generic;

namespace pbl3_QLCF.Data;

public partial class CaLamViec
{
    public string MaCaLamViec { get; set; } = null!;

    public string? TenCa { get; set; }

    public TimeOnly? GioBatDau { get; set; }

    public TimeOnly? GioKetThuc { get; set; }

    public string? MoTa { get; set; }

    public virtual ICollection<PhanCongCaLamViec> PhanCongCaLamViecs { get; set; } = new List<PhanCongCaLamViec>();
}
