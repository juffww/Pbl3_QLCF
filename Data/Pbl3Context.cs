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

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }
    public virtual DbSet<ThucDon> ThucDons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-B526NK5\\SQLEXPRESS;Initial Catalog=pbl3;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ban>(entity =>
        {
            entity.HasKey(e => e.MaBan).HasName("PK__Ban__3520ED6C422C19FA");

            //entity.ToTable("Ban");
            entity.ToTable("Ban", t => t.ExcludeFromMigrations());
            entity.Property(e => e.MaBan).HasMaxLength(20);
            entity.Property(e => e.KhuVucBan).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasMaxLength(20);
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
