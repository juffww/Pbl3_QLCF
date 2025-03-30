using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pbl3_QLCF.Data;

[Table("ThucDon")] 
public partial class ThucDon
{
    [Key] 
    [Required(ErrorMessage = "Mã món không được để trống")]
    [StringLength(10, ErrorMessage = "Mã món không quá 10 ký tự")]
    [Display(Name = "Mã Món")]
    public string MaMon { get; set; } = null!;

    [Required(ErrorMessage = "Tên món không được để trống")]
    [StringLength(100, ErrorMessage = "Tên món không quá 100 ký tự")]
    [Display(Name = "Tên Món")]
    public string TenMon { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn loại")]
    [StringLength(50, ErrorMessage = "Tên loại không quá 50 ký tự")]
    [Display(Name = "Loại")]
    public string? TenLoai { get; set; }

    [Url(ErrorMessage = "URL hình ảnh không hợp lệ")]
    [StringLength(255, ErrorMessage = "Đường dẫn hình ảnh quá dài")]
    [Display(Name = "Hình Ảnh")]
    public string? HinhAnh { get; set; }

    [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
    [Range(1000, int.MaxValue, ErrorMessage = "Giá sản phẩm phải từ 1,000 VNĐ trở lên")]
    [Display(Name = "Giá")]
    [DataType(DataType.Currency)]
    public int GiaSp { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
    [Display(Name = "Trạng Thái")]
    public bool? TinhTrang { get; set; } = true; 

    // Navigation properties
    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    public virtual ICollection<CongThucMonAn> CongThucMonAns { get; set; } = new List<CongThucMonAn>();
    public virtual ICollection<KhuyenMai> MaKms { get; set; } = new List<KhuyenMai>();

    // Có thể thêm các phương thức tiện ích
    public string TrangThaiDisplay => TinhTrang == true ? "Đang bán" : "Ngừng bán";

    public string GiaFormatted => GiaSp.ToString("N0") + " VNĐ";
}