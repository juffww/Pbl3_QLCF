using System;
using System.Collections.Generic;

namespace pbl3_QLCF.Data;

public partial class NguoiDung
{
    public string MaNv { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public bool? GioiTinh { get; set; }

    public string? Sdt { get; set; }

    public string? DiaChi { get; set; }

    public string? Email { get; set; }

    public string? ChucVu { get; set; }

    public string? TenDangNhap { get; set; }

    public string? MatKhau { get; set; }

    public string? CaLamViec { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<PhanCongCaLamViec> PhanCongCaLamViecs { get; set; } = new List<PhanCongCaLamViec>();
}
