using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pbl3_QLCF.Migrations
{
    /// <inheritdoc />
    public partial class FinalDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "KhoNguyenLieu");

            migrationBuilder.DropTable(
                name: "CaLamViec");

            migrationBuilder.DropTable(
                name: "NhaCungCap");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaLamViec",
                columns: table => new
                {
                    MaCaLamViec = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GioBatDau = table.Column<TimeOnly>(type: "time", nullable: true),
                    GioKetThuc = table.Column<TimeOnly>(type: "time", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TenCa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CaLamVie__E545F62584269E71", x => x.MaCaLamViec);
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    MaHD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaDH = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: true),
                    TongTien = table.Column<double>(type: "float", nullable: true),
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
                name: "KhuyenMai",
                columns: table => new
                {
                    MaKM = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DKApDung = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GiaTriGiam = table.Column<double>(type: "float", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayBD = table.Column<DateOnly>(type: "date", nullable: true),
                    NgayKT = table.Column<DateOnly>(type: "date", nullable: true),
                    TenKM = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KhuyenMa__2725CF15FFCBA44B", x => x.MaKM);
                });

            migrationBuilder.CreateTable(
                name: "NhaCungCap",
                columns: table => new
                {
                    MaNCC = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SDT = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    TenNCC = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NhaCungC__3A185DEB6CA7E147", x => x.MaNCC);
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
                name: "KhoNguyenLieu",
                columns: table => new
                {
                    MaNL = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaNhaCungCap = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DonViTinh = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    HSD = table.Column<DateOnly>(type: "date", nullable: true),
                    NgayNhap = table.Column<DateOnly>(type: "date", nullable: true),
                    NSX = table.Column<DateOnly>(type: "date", nullable: true),
                    SL = table.Column<int>(type: "int", nullable: true),
                    TenNL = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                name: "IX_ChiTietKhuyenMai_MaMon",
                table: "ChiTietKhuyenMai",
                column: "MaMon");

            migrationBuilder.CreateIndex(
                name: "IX_CongThucMonAn_MaNL",
                table: "CongThucMonAn",
                column: "MaNL");

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
    }
}
