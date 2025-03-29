using System;
using System.Collections.Generic;

namespace pbl3_QLCF.Data;

public partial class DonHang
{
    public string MaDh { get; set; } = null!;

    public string? MaKh { get; set; }

    public string? MaNv { get; set; }

    public string? MaBan { get; set; }

    public DateTime? ThoiGianDat { get; set; }

    public double? TongTien { get; set; }

    public string? ThanhToan { get; set; }

    public string? TrangThaiDh { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual Ban? MaBanNavigation { get; set; }

    public virtual KhachHang? MaKhNavigation { get; set; }

    public virtual NguoiDung? MaNvNavigation { get; set; }
}
