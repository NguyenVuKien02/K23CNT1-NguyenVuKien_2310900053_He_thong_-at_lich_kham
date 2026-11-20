using System;
using System.Collections.Generic;
using PhongKham.Common.Enums;

namespace PhongKham.Data.Entities;

public partial class ThongBao
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? LichHenId { get; set; }

    public string TieuDe { get; set; } = null!;

    public string? NoiDung { get; set; }

    public LoaiThongBao LoaiThongBao { get; set; }

    public bool DaDoc { get; set; }

    public bool DaGuiEmail { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual LichHen? LichHen { get; set; }

    public virtual User User { get; set; } = null!;
}
