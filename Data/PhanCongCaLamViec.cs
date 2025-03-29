using System;
using System.Collections.Generic;

namespace pbl3_QLCF.Data;

public partial class PhanCongCaLamViec
{
    public string MaNv { get; set; } = null!;

    public string MaCaLamViec { get; set; } = null!;

    public DateOnly NgayLamViec { get; set; }

    public virtual CaLamViec MaCaLamViecNavigation { get; set; } = null!;

    public virtual NguoiDung MaNvNavigation { get; set; } = null!;
}
