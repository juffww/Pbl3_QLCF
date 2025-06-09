using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pbl3_QLCF.Data
{
    public partial class NguoiDung
    {
        [Required(ErrorMessage = "Mã nhân viên là bắt buộc")]
        [StringLength(20, ErrorMessage = "Mã nhân viên không được vượt quá 20 ký tự")]
        [Display(Name = "Mã nhân viên")]
        public string MaNv { get; set; } = null!;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ tên phải có từ 2 đến 100 ký tự")]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = null!;

        [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
        [Display(Name = "Ngày sinh")]
        public DateOnly? NgaySinh { get; set; }

        [Required(ErrorMessage = "Giới tính là bắt buộc")]
        [Display(Name = "Giới tính")]
        public bool GioiTinh { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Số điện thoại phải có đúng 10 chữ số")]
        [Display(Name = "Số điện thoại")]
        public string Sdt { get; set; } = null!;

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Địa chỉ phải có từ 10 đến 200 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; } = null!;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Chức vụ là bắt buộc")]
        [Display(Name = "Chức vụ")]
        public string ChucVu { get; set; } = null!;

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải có từ 3 đến 50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = null!;

        [Required(ErrorMessage = "Ca làm việc là bắt buộc")]
        [Display(Name = "Ca làm việc")]
        public string CaLamViec { get; set; } = null!;

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = null!;

        public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
    }
}