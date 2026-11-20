using System;
using System.Collections.Generic;
using PhongKham.Common.Enums;

namespace PhongKham.Data.Entities;

public partial class LichSuLichHen
{
    public int Id { get; set; }

    public int LichHenId { get; set; }

    public TrangThaiLichHen? TrangThaiCu { get; set; }

    public TrangThaiLichHen TrangThaiMoi { get; set; }

    public int? NguoiThayDoiId { get; set; }

    public string? GhiChu { get; set; }

    public DateTime ThoiGian { get; set; }

    public virtual LichHen LichHen { get; set; } = null!;

    public virtual User? NguoiThayDoi { get; set; }
}
