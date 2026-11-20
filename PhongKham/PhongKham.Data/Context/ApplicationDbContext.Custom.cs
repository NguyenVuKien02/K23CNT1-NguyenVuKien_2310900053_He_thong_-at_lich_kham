using Microsoft.EntityFrameworkCore;
using PhongKham.Data.Entities;

namespace PhongKham.Data.Context;

public partial class ApplicationDbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiDinhDichVu>(entity =>
        {
            entity.Property(e => e.TrangThai).HasConversion<byte>();
        });

        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.Property(e => e.LoaiMuc).HasConversion<byte>();
        });

        modelBuilder.Entity<DichVu>(entity =>
        {
            entity.Property(e => e.LoaiDichVu).HasConversion<byte>();
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.Property(e => e.Gender).HasConversion<byte>();
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.Property(e => e.PhuongThucThanhToan).HasConversion<byte>();
            entity.Property(e => e.TrangThai).HasConversion<byte>();
        });

        modelBuilder.Entity<LichHen>(entity =>
        {
            entity.Property(e => e.HinhThucDat).HasConversion<byte>();
            entity.Property(e => e.TrangThai).HasConversion<byte>();
        });

        modelBuilder.Entity<LichSuLichHen>(entity =>
        {
            entity.Property(e => e.TrangThaiCu).HasConversion<byte>();
            entity.Property(e => e.TrangThaiMoi).HasConversion<byte>();
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.Property(e => e.Gender).HasConversion<byte>();
        });

        modelBuilder.Entity<PhieuKham>(entity =>
        {
            entity.Property(e => e.TrangThai).HasConversion<byte>();
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.Property(e => e.Gender).HasConversion<byte>();
        });

        modelBuilder.Entity<ThongBao>(entity =>
        {
            entity.Property(e => e.LoaiThongBao).HasConversion<byte>();
        });
    }
}
