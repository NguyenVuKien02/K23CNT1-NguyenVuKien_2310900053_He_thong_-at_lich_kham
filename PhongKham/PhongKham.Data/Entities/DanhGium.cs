using System;
using System.Collections.Generic;

namespace PhongKham.Data.Entities;

public partial class DanhGium
{
    public int Id { get; set; }

    public int LichHenId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public byte SoSao { get; set; }

    public string? NhanXet { get; set; }

    public DateTime NgayDanhGia { get; set; }

    public bool HienThi { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual LichHen LichHen { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
