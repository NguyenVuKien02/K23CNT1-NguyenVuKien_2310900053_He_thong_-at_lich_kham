using System;
using System.Collections.Generic;
using PhongKham.Common.Enums;

namespace PhongKham.Data.Entities;

public partial class ChiTietHoaDon
{
    public int Id { get; set; }

    public int HoaDonId { get; set; }

    public LoaiMucHoaDon LoaiMuc { get; set; }

    public string TenMuc { get; set; } = null!;

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public decimal? ThanhTien { get; set; }

    public virtual HoaDon HoaDon { get; set; } = null!;
}
