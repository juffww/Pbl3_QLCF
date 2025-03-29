using System;
using System.Collections.Generic;

namespace pbl3_QLCF.Data;

public partial class NhaCungCap
{
    public string MaNcc { get; set; } = null!;

    public string TenNcc { get; set; } = null!;

    public string? Sdt { get; set; }

    public string? Email { get; set; }

    public string? DiaChi { get; set; }

    public virtual ICollection<KhoNguyenLieu> KhoNguyenLieus { get; set; } = new List<KhoNguyenLieu>();
}
