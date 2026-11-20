using System;
using System.Collections.Generic;
using PhongKham.Common.Enums;

namespace PhongKham.Data.Entities;

public partial class ChiDinhDichVu
{
    public int Id { get; set; }

    public int PhieuKhamId { get; set; }

    public int DichVuId { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public TrangThaiChiDinh TrangThai { get; set; }

    public string? KetQua { get; set; }

    public string? FileKetQua { get; set; }

    public DateTime NgayChiDinh { get; set; }

    public DateTime? NgayCoKetQua { get; set; }

    public string? GhiChu { get; set; }

    public virtual DichVu DichVu { get; set; } = null!;

    public virtual PhieuKham PhieuKham { get; set; } = null!;
}
