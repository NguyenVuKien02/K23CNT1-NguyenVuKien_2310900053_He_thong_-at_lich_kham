using System;
using System.Collections.Generic;

namespace PhongKham.Data.Entities;

public partial class LichLamViec
{
    public int Id { get; set; }

    public int DoctorId { get; set; }

    public int CaLamViecId { get; set; }

    public DateOnly NgayLam { get; set; }

    public int SoBenhNhanToiDa { get; set; }

    public bool ConHieuLuc { get; set; }

    public string? GhiChu { get; set; }

    public virtual CaLamViec CaLamViec { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual ICollection<LichHen> LichHens { get; set; } = new List<LichHen>();
}
