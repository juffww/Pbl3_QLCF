using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace pbl3_QLCF.Data;

public partial class Pbl3Context : DbContext
{
    public Pbl3Context()
    {
    }

    public Pbl3Context(DbContextOptions<Pbl3Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Ban> Bans { get; set; }

    public virtual DbSet<CaLamViec> CaLamViecs { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<CongThucMonAn> CongThucMonAns { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<KhoNguyenLieu> KhoNguyenLieus { get; set; }

    public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<PhanCongCaLamViec> PhanCongCaLamViecs { get; set; }

    public virtual DbSet<ThucDon> ThucDons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-B526NK5\\SQLEXPRESS;Initial Catalog=pbl3;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ban>(entity =>
        {
            entity.HasKey(e => e.MaBan).HasName("PK__Ban__3520ED6C422C19FA");

            entity.ToTable("Ban");

            entity.Property(e => e.MaBan).HasMaxLength(20);
            entity.Property(e => e.KhuVucBan).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasMaxLength(20);
        });

        modelBuilder.Entity<CaLamViec>(entity =>
        {
            entity.HasKey(e => e.MaCaLamViec).HasName("PK__CaLamVie__E545F62584269E71");

            entity.ToTable("CaLamViec");

            entity.Property(e => e.MaCaLamViec).HasMaxLength(20);
            entity.Property(e => e.MoTa).HasMaxLength(200);
            entity.Property(e => e.TenCa).HasMaxLength(50);
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => new { e.MaDh, e.MaMon }).HasName("PK__ChiTietD__B48034FBD6C60B2C");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.MaDh)
                .HasMaxLength(20)
                .HasColumnName("MaDH");
            entity.Property(e => e.MaMon).HasMaxLength(20);
            entity.Property(e => e.GhiChu).HasMaxLength(200);

            entity.HasOne(d => d.MaDhNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDon__MaDH__09A971A2");

            entity.HasOne(d => d.MaMonNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaMon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__MaMon__0A9D95DB");
        });

        modelBuilder.Entity<CongThucMonAn>(entity =>
        {
            entity.HasKey(e => new { e.MaMon, e.MaNl }).HasName("PK__CongThuc__E82974DB2E3B7E9F");

            entity.ToTable("CongThucMonAn");

            entity.Property(e => e.MaMon).HasMaxLength(20);
            entity.Property(e => e.MaNl)
                .HasMaxLength(20)
                .HasColumnName("MaNL");

            entity.HasOne(d => d.MaMonNavigation).WithMany(p => p.CongThucMonAns)
                .HasForeignKey(d => d.MaMon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CongThucM__MaMon__6EF57B66");

            entity.HasOne(d => d.MaNlNavigation).WithMany(p => p.CongThucMonAns)
                .HasForeignKey(d => d.MaNl)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CongThucMo__MaNL__6FE99F9F");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDh).HasName("PK__DonHang__27258661D23B0614");

            entity.ToTable("DonHang");

            entity.Property(e => e.MaDh)
                .HasMaxLength(20)
                .HasColumnName("MaDH");
            entity.Property(e => e.MaBan).HasMaxLength(20);
            entity.Property(e => e.MaKh)
                .HasMaxLength(20)
                .HasColumnName("MaKH");
            entity.Property(e => e.MaNv)
                .HasMaxLength(20)
                .HasColumnName("MaNV");
            entity.Property(e => e.ThanhToan).HasMaxLength(20);
            entity.Property(e => e.ThoiGianDat).HasColumnType("datetime");
            entity.Property(e => e.TrangThaiDh)
                .HasMaxLength(20)
                .HasColumnName("TrangThaiDH");

            entity.HasOne(d => d.MaBanNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaBan)
                .HasConstraintName("FK__DonHang__MaBan__06CD04F7");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaKh)
                .HasConstraintName("FK__DonHang__MaKH__04E4BC85");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaNv)
                .HasConstraintName("FK__DonHang__MaNV__05D8E0BE");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK__HoaDon__2725A6E040E95A9D");

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHd)
                .HasMaxLength(20)
                .HasColumnName("MaHD");
            entity.Property(e => e.MaDh)
                .HasMaxLength(20)
                .HasColumnName("MaDH");

            entity.HasOne(d => d.MaDhNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaDh)
                .HasConstraintName("FK__HoaDon__MaDH__0D7A0286");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KhachHan__2725CF1E13284CE0");

            entity.ToTable("KhachHang");

            entity.Property(e => e.MaKh)
                .HasMaxLength(20)
                .HasColumnName("MaKH");
            entity.Property(e => e.DiemTichLuy).HasDefaultValue(0);
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .HasColumnName("SDT");
            entity.Property(e => e.TenKh)
                .HasMaxLength(100)
                .HasColumnName("TenKH");
        });

        modelBuilder.Entity<KhoNguyenLieu>(entity =>
        {
            entity.HasKey(e => e.MaNl).HasName("PK__KhoNguye__2725D73C7BD649CA");

            entity.ToTable("KhoNguyenLieu");

            entity.Property(e => e.MaNl)
                .HasMaxLength(20)
                .HasColumnName("MaNL");
            entity.Property(e => e.DonViTinh).HasMaxLength(20);
            entity.Property(e => e.Hsd).HasColumnName("HSD");
            entity.Property(e => e.MaNhaCungCap).HasMaxLength(20);
            entity.Property(e => e.Nsx).HasColumnName("NSX");
            entity.Property(e => e.Sl).HasColumnName("SL");
            entity.Property(e => e.TenNl)
                .HasMaxLength(100)
                .HasColumnName("TenNL");

            entity.HasOne(d => d.MaNhaCungCapNavigation).WithMany(p => p.KhoNguyenLieus)
                .HasForeignKey(d => d.MaNhaCungCap)
                .HasConstraintName("FK__KhoNguyen__MaNha__6A30C649");
        });

        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.MaKm).HasName("PK__KhuyenMa__2725CF15FFCBA44B");

            entity.ToTable("KhuyenMai");

            entity.Property(e => e.MaKm)
                .HasMaxLength(20)
                .HasColumnName("MaKM");
            entity.Property(e => e.DkapDung)
                .HasMaxLength(200)
                .HasColumnName("DKApDung");
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.NgayBd).HasColumnName("NgayBD");
            entity.Property(e => e.NgayKt).HasColumnName("NgayKT");
            entity.Property(e => e.TenKm)
                .HasMaxLength(100)
                .HasColumnName("TenKM");

            entity.HasMany(d => d.MaMons).WithMany(p => p.MaKms)
                .UsingEntity<Dictionary<string, object>>(
                    "ChiTietKhuyenMai",
                    r => r.HasOne<ThucDon>().WithMany()
                        .HasForeignKey("MaMon")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ChiTietKh__MaMon__75A278F5"),
                    l => l.HasOne<KhuyenMai>().WithMany()
                        .HasForeignKey("MaKm")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ChiTietKhu__MaKM__74AE54BC"),
                    j =>
                    {
                        j.HasKey("MaKm", "MaMon").HasName("PK__ChiTietK__B4807D8F8F30185A");
                        j.ToTable("ChiTietKhuyenMai");
                        j.IndexerProperty<string>("MaKm")
                            .HasMaxLength(20)
                            .HasColumnName("MaKM");
                        j.IndexerProperty<string>("MaMon").HasMaxLength(20);
                    });
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NguoiDun__2725D70A258C1FBD");

            entity.ToTable("NguoiDung");

            entity.Property(e => e.MaNv)
                .HasMaxLength(20)
                .HasColumnName("MaNV");
            entity.Property(e => e.CaLamViec).HasMaxLength(50);
            entity.Property(e => e.ChucVu).HasMaxLength(50);
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(100);
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .HasColumnName("SDT");
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasMaxLength(20);
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.MaNcc).HasName("PK__NhaCungC__3A185DEB6CA7E147");

            entity.ToTable("NhaCungCap");

            entity.Property(e => e.MaNcc)
                .HasMaxLength(20)
                .HasColumnName("MaNCC");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .HasColumnName("SDT");
            entity.Property(e => e.TenNcc)
                .HasMaxLength(100)
                .HasColumnName("TenNCC");
        });

        modelBuilder.Entity<PhanCongCaLamViec>(entity =>
        {
            entity.HasKey(e => new { e.MaNv, e.MaCaLamViec, e.NgayLamViec }).HasName("PK__PhanCong__D543FA9A746C1BC8");

            entity.ToTable("PhanCongCaLamViec");

            entity.Property(e => e.MaNv)
                .HasMaxLength(20)
                .HasColumnName("MaNV");
            entity.Property(e => e.MaCaLamViec).HasMaxLength(20);

            entity.HasOne(d => d.MaCaLamViecNavigation).WithMany(p => p.PhanCongCaLamViecs)
                .HasForeignKey(d => d.MaCaLamViec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhanCongC__MaCaL__00200768");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.PhanCongCaLamViecs)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhanCongCa__MaNV__7F2BE32F");
        });

        modelBuilder.Entity<ThucDon>(entity =>
        {
            entity.HasKey(e => e.MaMon).HasName("PK__ThucDon__3A5B29A8C030D407");

            entity.ToTable("ThucDon");

            entity.Property(e => e.MaMon).HasMaxLength(20);
            entity.Property(e => e.GiaSp).HasColumnName("GiaSP");
            entity.Property(e => e.HinhAnh).HasMaxLength(500);
            entity.Property(e => e.TenLoai).HasMaxLength(500);
            entity.Property(e => e.TenMon).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
