using System;
using System.Collections.Generic;
using PhongKham.Common.Enums;

namespace PhongKham.Data.Entities;

public partial class DichVu
{
    public int Id { get; set; }

    public string MaDichVu { get; set; } = null!;

    public string TenDichVu { get; set; } = null!;

    public LoaiDichVu LoaiDichVu { get; set; }

    public decimal DonGia { get; set; }

    public string? MoTa { get; set; }

    public bool ConHieuLuc { get; set; }

    public virtual ICollection<ChiDinhDichVu> ChiDinhDichVus { get; set; } = new List<ChiDinhDichVu>();
}
