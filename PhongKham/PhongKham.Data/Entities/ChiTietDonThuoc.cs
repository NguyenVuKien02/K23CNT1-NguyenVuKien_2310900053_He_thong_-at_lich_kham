using System;
using System.Collections.Generic;

namespace PhongKham.Data.Entities;

public partial class ChiTietDonThuoc
{
    public int Id { get; set; }

    public int DonThuocId { get; set; }

    public int MedicineId { get; set; }

    public int SoLuong { get; set; }

    public string? LieuDung { get; set; }

    public string? CachDung { get; set; }

    public int? SoNgayDung { get; set; }

    public decimal DonGia { get; set; }

    public string? GhiChu { get; set; }

    public virtual DonThuoc DonThuoc { get; set; } = null!;

    public virtual Medicine Medicine { get; set; } = null!;
}
