using pbl3_QLCF.Data;
using System.Collections.Generic;

namespace pbl3_QLCF.ViewModels
{
    public class CTDHViewModel
    {
        //Order info
        public string MaDh { get; set; }
        public DateTime? ThoigianDat { get; set; }
        public string TrangThaiDh { get; set; }
        public int? TongTien { get; set; }
        public string ThanhToan { get; set; }
        //NV info
        public string? MaNv { get; set; }
        public string? tenNv { get; set; }
        public string? MaBan {  get; set; }
        //KH info
        public string MaKh { get; set; }
        public string tenKh { get; set; }
        public string SDT { get; set; }
        //orderDetail
        public List<ChiTietDonHang>? CTDHs { get; set; } = new List<ChiTietDonHang>();
        //
        public int? Giam { get; set; }
    }
}
