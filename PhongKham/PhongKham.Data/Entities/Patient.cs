using System;
using System.Collections.Generic;
using PhongKham.Common.Enums;

namespace PhongKham.Data.Entities;

public partial class Patient
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string PatientCode { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public GioiTinh? Gender { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? IdentityNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual HoSoBenhAn? HoSoBenhAn { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual ICollection<LichHen> LichHens { get; set; } = new List<LichHen>();

    public virtual User? User { get; set; }
}
