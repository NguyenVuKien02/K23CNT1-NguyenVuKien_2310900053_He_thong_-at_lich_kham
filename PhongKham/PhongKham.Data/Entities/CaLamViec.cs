using System;
using System.Collections.Generic;

namespace PhongKham.Data.Entities;

public partial class CaLamViec
{
    public int Id { get; set; }

    public string TenCa { get; set; } = null!;

    public TimeOnly GioBatDau { get; set; }

    public TimeOnly GioKetThuc { get; set; }

    public int ThoiLuongKhungGio { get; set; }

    public virtual ICollection<LichLamViec> LichLamViecs { get; set; } = new List<LichLamViec>();
}
