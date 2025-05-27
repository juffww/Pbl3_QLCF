using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pbl3_QLCF.Data;

public partial class NguoiDung
{
    public string MaNv { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public bool? GioiTinh { get; set; }

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [RegularExpression(@"^(\+84|84|0[3|5|7|8|9])[0-9]{8,9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại")]
    public string? Sdt { get; set; }

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
    [StringLength(200, MinimumLength = 10, ErrorMessage = "Địa chỉ phải có từ 10 đến 200 ký tự")]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    public string? ChucVu { get; set; }

    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải có từ 3 đến 50 ký tự")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới")]
    [Display(Name = "Tên đăng nhập")]
    public string? TenDangNhap { get; set; }

    public string? MatKhau { get; set; }

    public string? CaLamViec { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

}
