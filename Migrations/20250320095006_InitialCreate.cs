using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pbl3_QLCF.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ban",
                columns: table => new
                {
                    MaBan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    KhuVucBan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ban__3520ED6C422C19FA", x => x.MaBan);
                });

            migrationBuilder.CreateTable(
                name: "CaLamViec",
                columns: table => new
                {
                    MaCaLamViec = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenCa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GioBatDau = table.Column<TimeOnly>(type: "time", nullable: true),
                    GioKetThuc = table.Column<TimeOnly>(type: "time", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CaLamVie__E545F62584269E71", x => x.MaCaLamViec);
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    MaKH = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenKH = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DiemTichLuy = table.Column<int>(type: "int", nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KhachHan__2725CF1E13284CE0", x => x.MaKH);
                });

            migrationBuilder.CreateTable(
                name: "KhuyenMai",
                columns: table => new
                {
                    MaKM = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenKM = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GiaTriGiam = table.Column<double>(type: "float", nullable: true),
                    NgayBD = table.Column<DateOnly>(type: "date", nullable: true),
                    NgayKT = table.Column<DateOnly>(type: "date", nullable: true),
                    DKApDung = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KhuyenMa__2725CF15FFCBA44B", x => x.MaKM);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    MaNV = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: true),
                    GioiTinh = table.Column<bool>(type: "bit", nullable: true),
                    SDT = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChucVu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MatKhau = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CaLamViec = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NguoiDun__2725D70A258C1FBD", x => x.MaNV);
                });

            migrationBuilder.CreateTable(
                name: "NhaCungCap",
                columns: table => new
                {
                    MaNCC = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenNCC = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NhaCungC__3A185DEB6CA7E147", x => x.MaNCC);
                });

            migrationBuilder.CreateTable(
                name: "ThucDon",
                columns: table => new
                {
                    MaMon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenMon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenLoai = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    GiaSP = table.Column<int>(type: "int", nullable: true),
                    TinhTrang = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ThucDon__3A5B29A8C030D407", x => x.MaMon);
                });

            migrationBuilder.CreateTable(
                name: "DonHang",
                columns: table => new
                {
                    MaDH = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaKH = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaNV = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaBan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ThoiGianDat = table.Column<DateTime>(type: "datetime", nullable: true),
                    TongTien = table.Column<double>(type: "float", nullable: true),
                    ThanhToan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TrangThaiDH = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DonHang__27258661D23B0614", x => x.MaDH);
                    table.ForeignKey(
                        name: "FK__DonHang__MaBan__06CD04F7",
                        column: x => x.MaBan,
                        principalTable: "Ban",
                        principalColumn: "MaBan");
                    table.ForeignKey(
                        name: "FK__DonHang__MaKH__04E4BC85",
                        column: x => x.MaKH,
                        principalTable: "KhachHang",
                        principalColumn: "MaKH");
                    table.ForeignKey(
                        name: "FK__DonHang__MaNV__05D8E0BE",
                        column: x => x.MaNV,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNV");
                });

            migrationBuilder.CreateTable(
                name: "PhanCongCaLamViec",
                columns: table => new
                {
                    MaNV = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaCaLamViec = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayLamViec = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PhanCong__D543FA9A746C1BC8", x => new { x.MaNV, x.MaCaLamViec, x.NgayLamViec });
                    table.ForeignKey(
                        name: "FK__PhanCongC__MaCaL__00200768",
                        column: x => x.MaCaLamViec,
                        principalTable: "CaLamViec",
                        principalColumn: "MaCaLamViec");
                    table.ForeignKey(
                        name: "FK__PhanCongCa__MaNV__7F2BE32F",
                        column: x => x.MaNV,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNV");
                });

            migrationBuilder.CreateTable(
                name: "KhoNguyenLieu",
                columns: table => new
                {
                    MaNL = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenNL = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgayNhap = table.Column<DateOnly>(type: "date", nullable: true),
                    HSD = table.Column<DateOnly>(type: "date", nullable: true),
                    NSX = table.Column<DateOnly>(type: "date", nullable: true),
                    MaNhaCungCap = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DonViTinh = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SL = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KhoNguye__2725D73C7BD649CA", x => x.MaNL);
                    table.ForeignKey(
                        name: "FK__KhoNguyen__MaNha__6A30C649",
                        column: x => x.MaNhaCungCap,
                        principalTable: "NhaCungCap",
                        principalColumn: "MaNCC");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietKhuyenMai",
                columns: table => new
                {
                    MaKM = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaMon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietK__B4807D8F8F30185A", x => new { x.MaKM, x.MaMon });
                    table.ForeignKey(
                        name: "FK__ChiTietKh__MaMon__75A278F5",
                        column: x => x.MaMon,
                        principalTable: "ThucDon",
                        principalColumn: "MaMon");
                    table.ForeignKey(
                        name: "FK__ChiTietKhu__MaKM__74AE54BC",
                        column: x => x.MaKM,
                        principalTable: "KhuyenMai",
                        principalColumn: "MaKM");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonHang",
                columns: table => new
                {
                    MaDH = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaMon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: true),
                    GiaBan = table.Column<double>(type: "float", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietD__B48034FBD6C60B2C", x => new { x.MaDH, x.MaMon });
                    table.ForeignKey(
                        name: "FK__ChiTietDo__MaMon__0A9D95DB",
                        column: x => x.MaMon,
                        principalTable: "ThucDon",
                        principalColumn: "MaMon");
                    table.ForeignKey(
                        name: "FK__ChiTietDon__MaDH__09A971A2",
                        column: x => x.MaDH,
                        principalTable: "DonHang",
                        principalColumn: "MaDH");
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    MaHD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaDH = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TongTien = table.Column<double>(type: "float", nullable: true),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HoaDon__2725A6E040E95A9D", x => x.MaHD);
                    table.ForeignKey(
                        name: "FK__HoaDon__MaDH__0D7A0286",
                        column: x => x.MaDH,
                        principalTable: "DonHang",
                        principalColumn: "MaDH");
                });

            migrationBuilder.CreateTable(
                name: "CongThucMonAn",
                columns: table => new
                {
                    MaMon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaNL = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoLuong = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CongThuc__E82974DB2E3B7E9F", x => new { x.MaMon, x.MaNL });
                    table.ForeignKey(
                        name: "FK__CongThucM__MaMon__6EF57B66",
                        column: x => x.MaMon,
                        principalTable: "ThucDon",
                        principalColumn: "MaMon");
                    table.ForeignKey(
                        name: "FK__CongThucMo__MaNL__6FE99F9F",
                        column: x => x.MaNL,
                        principalTable: "KhoNguyenLieu",
                        principalColumn: "MaNL");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_MaMon",
                table: "ChiTietDonHang",
                column: "MaMon");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietKhuyenMai_MaMon",
                table: "ChiTietKhuyenMai",
                column: "MaMon");

            migrationBuilder.CreateIndex(
                name: "IX_CongThucMonAn_MaNL",
                table: "CongThucMonAn",
                column: "MaNL");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaBan",
                table: "DonHang",
                column: "MaBan");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaKH",
                table: "DonHang",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaNV",
                table: "DonHang",
                column: "MaNV");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaDH",
                table: "HoaDon",
                column: "MaDH");

            migrationBuilder.CreateIndex(
                name: "IX_KhoNguyenLieu_MaNhaCungCap",
                table: "KhoNguyenLieu",
                column: "MaNhaCungCap");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongCaLamViec_MaCaLamViec",
                table: "PhanCongCaLamViec",
                column: "MaCaLamViec");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietDonHang");

            migrationBuilder.DropTable(
                name: "ChiTietKhuyenMai");

            migrationBuilder.DropTable(
                name: "CongThucMonAn");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "PhanCongCaLamViec");

            migrationBuilder.DropTable(
                name: "KhuyenMai");

            migrationBuilder.DropTable(
                name: "ThucDon");

            migrationBuilder.DropTable(
                name: "KhoNguyenLieu");

            migrationBuilder.DropTable(
                name: "DonHang");

            migrationBuilder.DropTable(
                name: "CaLamViec");

            migrationBuilder.DropTable(
                name: "NhaCungCap");

            migrationBuilder.DropTable(
                name: "Ban");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "NguoiDung");
        }
    }
}
