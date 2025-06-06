using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pbl3_QLCF.Data;

public partial class KhachHang
{
    public string MaKh { get; set; } = null!;

    public string TenKh { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [RegularExpression(@"^(\+84|84|0[3|5|7|8|9])[0-9]{8,9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại")]
    public string Sdt { get; set; }

    public int? DiemTichLuy { get; set; }
    public string? LoaiKH {  get; set; }
    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
