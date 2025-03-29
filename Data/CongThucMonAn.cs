using System;
using System.Collections.Generic;

namespace pbl3_QLCF.Data;

public partial class CongThucMonAn
{
    public string MaMon { get; set; } = null!;

    public string MaNl { get; set; } = null!;

    public double? SoLuong { get; set; }

    public virtual ThucDon MaMonNavigation { get; set; } = null!;

    public virtual KhoNguyenLieu MaNlNavigation { get; set; } = null!;
}
