using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PhongKham.Data.Entities;

namespace PhongKham.Data.Context;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CaLamViec> CaLamViecs { get; set; }

    public virtual DbSet<ChiDinhDichVu> ChiDinhDichVus { get; set; }

    public virtual DbSet<ChiTietDonThuoc> ChiTietDonThuocs { get; set; }

    public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

    public virtual DbSet<DanhGium> DanhGia { get; set; }

    public virtual DbSet<DichVu> DichVus { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DonThuoc> DonThuocs { get; set; }

    public virtual DbSet<HoSoBenhAn> HoSoBenhAns { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<LichHen> LichHens { get; set; }

    public virtual DbSet<LichLamViec> LichLamViecs { get; set; }

    public virtual DbSet<LichSuLichHen> LichSuLichHens { get; set; }

    public virtual DbSet<Medicine> Medicines { get; set; }

    public virtual DbSet<MedicineCategory> MedicineCategories { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<PhieuKham> PhieuKhams { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Specialty> Specialties { get; set; }

    public virtual DbSet<Staff> Staffs { get; set; }

    public virtual DbSet<ThongBao> ThongBaos { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Vietnamese_CI_AS");

        modelBuilder.Entity<CaLamViec>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CaLamVie__3214EC07C873BC2F");

            entity.ToTable("CaLamViec");

            entity.Property(e => e.TenCa).HasMaxLength(50);
            entity.Property(e => e.ThoiLuongKhungGio).HasDefaultValue(30);
        });

        modelBuilder.Entity<ChiDinhDichVu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiDinhD__3214EC078DF1F45D");

            entity.ToTable("ChiDinhDichVu");

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.FileKetQua).HasMaxLength(300);
            entity.Property(e => e.GhiChu).HasMaxLength(300);
            entity.Property(e => e.KetQua).HasMaxLength(2000);
            entity.Property(e => e.NgayChiDinh).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.SoLuong).HasDefaultValue(1);

            entity.HasOne(d => d.DichVu).WithMany(p => p.ChiDinhDichVus)
                .HasForeignKey(d => d.DichVuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiDinhDi__DichV__1EA48E88");

            entity.HasOne(d => d.PhieuKham).WithMany(p => p.ChiDinhDichVus)
                .HasForeignKey(d => d.PhieuKhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiDinhDi__Phieu__1DB06A4F");
        });

        modelBuilder.Entity<ChiTietDonThuoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiTietD__3214EC074C864AD1");

            entity.ToTable("ChiTietDonThuoc");

            entity.Property(e => e.CachDung).HasMaxLength(200);
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.GhiChu).HasMaxLength(300);
            entity.Property(e => e.LieuDung).HasMaxLength(100);

            entity.HasOne(d => d.DonThuoc).WithMany(p => p.ChiTietDonThuocs)
                .HasForeignKey(d => d.DonThuocId)
                .HasConstraintName("FK__ChiTietDo__DonTh__114A936A");

            entity.HasOne(d => d.Medicine).WithMany(p => p.ChiTietDonThuocs)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__Medic__123EB7A3");
        });

        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiTietH__3214EC07FEF78F8F");

            entity.ToTable("ChiTietHoaDon");

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.SoLuong).HasDefaultValue(1);
            entity.Property(e => e.TenMuc).HasMaxLength(200);
            entity.Property(e => e.ThanhTien)
                .HasComputedColumnSql("([SoLuong]*[DonGia])", true)
                .HasColumnType("decimal(29, 0)");

            entity.HasOne(d => d.HoaDon).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.HoaDonId)
                .HasConstraintName("FK__ChiTietHo__HoaDo__3493CFA7");
        });

        modelBuilder.Entity<DanhGium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DanhGia__3214EC07E39C4008");

            entity.HasIndex(e => e.LichHenId, "UQ__DanhGia__F929919762136F87").IsUnique();

            entity.Property(e => e.HienThi).HasDefaultValue(true);
            entity.Property(e => e.NgayDanhGia).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.NhanXet).HasMaxLength(1000);

            entity.HasOne(d => d.Doctor).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGia__DoctorI__3E1D39E1");

            entity.HasOne(d => d.LichHen).WithOne(p => p.DanhGium)
                .HasForeignKey<DanhGium>(d => d.LichHenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGia__LichHen__3C34F16F");

            entity.HasOne(d => d.Patient).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGia__Patient__3D2915A8");
        });

        modelBuilder.Entity<DichVu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DichVu__3214EC07940C920D");

            entity.ToTable("DichVu");

            entity.HasIndex(e => e.MaDichVu, "UQ__DichVu__C0E6DE8EC13AB396").IsUnique();

            entity.Property(e => e.ConHieuLuc).HasDefaultValue(true);
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.MaDichVu)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.TenDichVu).HasMaxLength(200);
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Doctors__3214EC070987E4A4");

            entity.HasIndex(e => e.UserId, "UQ__Doctors__1788CC4D7DE76289").IsUnique();

            entity.Property(e => e.AvatarUrl).HasMaxLength(300);
            entity.Property(e => e.Bio).HasMaxLength(1000);
            entity.Property(e => e.ConsultationFee).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Degree).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.Specialty).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.SpecialtyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Doctors__Special__45F365D3");

            entity.HasOne(d => d.User).WithOne(p => p.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Doctors__UserId__44FF419A");
        });

        modelBuilder.Entity<DonThuoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DonThuoc__3214EC07B992A198");

            entity.ToTable("DonThuoc");

            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.NgayKeDon).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.PhieuKham).WithMany(p => p.DonThuocs)
                .HasForeignKey(d => d.PhieuKhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonThuoc__PhieuK__02FC7413");
        });

        modelBuilder.Entity<HoSoBenhAn>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HoSoBenh__3214EC073E9EA2CD");

            entity.ToTable("HoSoBenhAn");

            entity.HasIndex(e => e.MaHoSo, "UQ__HoSoBenh__1666423DF70C2E0D").IsUnique();

            entity.HasIndex(e => e.PatientId, "UQ__HoSoBenh__970EC3676C682D9F").IsUnique();

            entity.Property(e => e.BenhManTinh).HasMaxLength(500);
            entity.Property(e => e.DiUng).HasMaxLength(500);
            entity.Property(e => e.MaHoSo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.NguoiLienHe).HasMaxLength(100);
            entity.Property(e => e.NhomMau)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.SdtNguoiLienHe)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TienSuBenh).HasMaxLength(1000);
            entity.Property(e => e.TienSuGiaDinh).HasMaxLength(500);

            entity.HasOne(d => d.Patient).WithOne(p => p.HoSoBenhAn)
                .HasForeignKey<HoSoBenhAn>(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoSoBenhA__Patie__76969D2E");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HoaDon__3214EC07A1582262");

            entity.ToTable("HoaDon");

            entity.HasIndex(e => e.MaHoaDon, "UQ__HoaDon__835ED13A7B8F8014").IsUnique();

            entity.HasIndex(e => e.PhieuKhamId, "UQ__HoaDon__8F051B68F20AA581").IsUnique();

            entity.Property(e => e.GiamGia).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.MaHoaDon)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NgayLap).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ThanhTien)
                .HasComputedColumnSql("([TongTien]-[GiamGia])", true)
                .HasColumnType("decimal(19, 0)");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.NguoiLap).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.NguoiLapId)
                .HasConstraintName("FK__HoaDon__NguoiLap__2B0A656D");

            entity.HasOne(d => d.Patient).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__PatientI__2A164134");

            entity.HasOne(d => d.PhieuKham).WithOne(p => p.HoaDon)
                .HasForeignKey<HoaDon>(d => d.PhieuKhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__PhieuKha__29221CFB");
        });

        modelBuilder.Entity<LichHen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LichHen__3214EC073B1E67F7");

            entity.ToTable("LichHen");

            entity.HasIndex(e => new { e.NgayHen, e.TrangThai }, "IX_LichHen_NgayHen");

            entity.HasIndex(e => new { e.PatientId, e.NgayHen }, "IX_LichHen_PatientId");

            entity.HasIndex(e => e.MaLichHen, "UQ__LichHen__150F264E349E9CBA").IsUnique();

            entity.HasIndex(e => new { e.DoctorId, e.NgayHen, e.GioBatDau }, "UX_LichHen_KhungGio")
                .IsUnique()
                .HasFilter("([TrangThai]<(5))");

            entity.Property(e => e.LyDoHuy).HasMaxLength(300);
            entity.Property(e => e.LyDoKham).HasMaxLength(500);
            entity.Property(e => e.MaLichHen)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Doctor).WithMany(p => p.LichHens)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LichHen__DoctorI__656C112C");

            entity.HasOne(d => d.LichLamViec).WithMany(p => p.LichHens)
                .HasForeignKey(d => d.LichLamViecId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LichHen__LichLam__66603565");

            entity.HasOne(d => d.NguoiTao).WithMany(p => p.LichHens)
                .HasForeignKey(d => d.NguoiTaoId)
                .HasConstraintName("FK__LichHen__NguoiTa__6B24EA82");

            entity.HasOne(d => d.Patient).WithMany(p => p.LichHens)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LichHen__Patient__6477ECF3");
        });

        modelBuilder.Entity<LichLamViec>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LichLamV__3214EC07337918F7");

            entity.ToTable("LichLamViec");

            entity.HasIndex(e => new { e.DoctorId, e.NgayLam, e.CaLamViecId }, "UQ_LichLamViec").IsUnique();

            entity.Property(e => e.ConHieuLuc).HasDefaultValue(true);
            entity.Property(e => e.GhiChu).HasMaxLength(200);
            entity.Property(e => e.SoBenhNhanToiDa).HasDefaultValue(8);

            entity.HasOne(d => d.CaLamViec).WithMany(p => p.LichLamViecs)
                .HasForeignKey(d => d.CaLamViecId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LichLamVi__CaLam__5DCAEF64");

            entity.HasOne(d => d.Doctor).WithMany(p => p.LichLamViecs)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LichLamVi__Docto__5CD6CB2B");
        });

        modelBuilder.Entity<LichSuLichHen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LichSuLi__3214EC0744D6CF95");

            entity.ToTable("LichSuLichHen");

            entity.Property(e => e.GhiChu).HasMaxLength(300);
            entity.Property(e => e.ThoiGian).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.LichHen).WithMany(p => p.LichSuLichHens)
                .HasForeignKey(d => d.LichHenId)
                .HasConstraintName("FK__LichSuLic__LichH__6FE99F9F");

            entity.HasOne(d => d.NguoiThayDoi).WithMany(p => p.LichSuLichHens)
                .HasForeignKey(d => d.NguoiThayDoiId)
                .HasConstraintName("FK__LichSuLic__Nguoi__70DDC3D8");
        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Medicine__3214EC0777DAC2FE");

            entity.Property(e => e.ActiveIngredient).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Manufacturer).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Unit).HasMaxLength(30);

            entity.HasOne(d => d.MedicineCategory).WithMany(p => p.Medicines)
                .HasForeignKey(d => d.MedicineCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Medicines__Medic__09A971A2");
        });

        modelBuilder.Entity<MedicineCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Medicine__3214EC074A929E7E");

            entity.HasIndex(e => e.Name, "UQ__Medicine__737584F6582723E5").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Patients__3214EC07402BD21E");

            entity.HasIndex(e => e.FullName, "IX_Patients_FullName");

            entity.HasIndex(e => e.Phone, "IX_Patients_Phone");

            entity.HasIndex(e => e.PatientCode, "UQ__Patients__B9C66DFE302D6257").IsUnique();

            entity.HasIndex(e => e.UserId, "UX_Patients_UserId")
                .IsUnique()
                .HasFilter("([UserId] IS NOT NULL)");

            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IdentityNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PatientCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithOne(p => p.Patient)
                .HasForeignKey<Patient>(d => d.UserId)
                .HasConstraintName("FK__Patients__UserId__534D60F1");
        });

        modelBuilder.Entity<PhieuKham>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PhieuKha__3214EC07FD935926");

            entity.ToTable("PhieuKham");

            entity.HasIndex(e => new { e.HoSoBenhAnId, e.NgayKham }, "IX_PhieuKham_HoSoBenhAnId").IsDescending(false, true);

            entity.HasIndex(e => e.MaPhieuKham, "UQ__PhieuKha__FACA55DE5D2ED179").IsUnique();

            entity.HasIndex(e => e.LichHenId, "UX_PhieuKham_LichHenId")
                .IsUnique()
                .HasFilter("([LichHenId] IS NOT NULL)");

            entity.Property(e => e.CanNang).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ChanDoan).HasMaxLength(1000);
            entity.Property(e => e.ChieuCao).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.KetLuan).HasMaxLength(1000);
            entity.Property(e => e.LoiDanBacSi).HasMaxLength(1000);
            entity.Property(e => e.MaPhieuKham)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NgayKham).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.NhietDo).HasColumnType("decimal(4, 1)");
            entity.Property(e => e.TrieuChung).HasMaxLength(1000);

            entity.HasOne(d => d.Doctor).WithMany(p => p.PhieuKhams)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhieuKham__Docto__7D439ABD");

            entity.HasOne(d => d.HoSoBenhAn).WithMany(p => p.PhieuKhams)
                .HasForeignKey(d => d.HoSoBenhAnId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhieuKham__HoSoB__7B5B524B");

            entity.HasOne(d => d.LichHen).WithOne(p => p.PhieuKham)
                .HasForeignKey<PhieuKham>(d => d.LichHenId)
                .HasConstraintName("FK__PhieuKham__LichH__7C4F7684");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC077764962A");

            entity.HasIndex(e => e.Name, "UQ__Roles__737584F696CDB70A").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Specialt__3214EC077E5CAFFD");

            entity.HasIndex(e => e.Name, "UQ__Specialt__737584F677A7D784").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Staffs__3214EC071D23CB3D");

            entity.HasIndex(e => e.UserId, "UQ__Staffs__1788CC4DF6F5B93F").IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Position).HasMaxLength(100);

            entity.HasOne(d => d.User).WithOne(p => p.Staff)
                .HasForeignKey<Staff>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Staffs__UserId__4D94879B");
        });

        modelBuilder.Entity<ThongBao>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ThongBao__3214EC07ED1F58B4");

            entity.ToTable("ThongBao");

            entity.HasIndex(e => new { e.UserId, e.DaDoc, e.NgayTao }, "IX_ThongBao_User").IsDescending(false, false, true);

            entity.Property(e => e.NgayTao).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.NoiDung).HasMaxLength(1000);
            entity.Property(e => e.TieuDe).HasMaxLength(200);

            entity.HasOne(d => d.LichHen).WithMany(p => p.ThongBaos)
                .HasForeignKey(d => d.LichHenId)
                .HasConstraintName("FK__ThongBao__LichHe__44CA3770");

            entity.HasOne(d => d.User).WithMany(p => p.ThongBaos)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ThongBao__UserId__43D61337");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0790647F9F");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534B8915697").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__3B75D760");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
