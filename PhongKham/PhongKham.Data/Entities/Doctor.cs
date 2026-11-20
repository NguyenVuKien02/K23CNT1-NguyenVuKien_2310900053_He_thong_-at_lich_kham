using System;
using System.Collections.Generic;
using PhongKham.Common.Enums;

namespace PhongKham.Data.Entities;

public partial class Doctor
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int SpecialtyId { get; set; }

    public string FullName { get; set; } = null!;

    public GioiTinh? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Phone { get; set; }

    public string? Degree { get; set; }

    public int? ExperienceYears { get; set; }

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    public decimal ConsultationFee { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual ICollection<LichHen> LichHens { get; set; } = new List<LichHen>();

    public virtual ICollection<LichLamViec> LichLamViecs { get; set; } = new List<LichLamViec>();

    public virtual ICollection<PhieuKham> PhieuKhams { get; set; } = new List<PhieuKham>();

    public virtual Specialty Specialty { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
