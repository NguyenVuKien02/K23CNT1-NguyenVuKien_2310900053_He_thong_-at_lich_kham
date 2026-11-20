using System;
using System.Collections.Generic;

namespace PhongKham.Data.Entities;

public partial class DonThuoc
{
    public int Id { get; set; }

    public int PhieuKhamId { get; set; }

    public DateTime NgayKeDon { get; set; }

    public string? GhiChu { get; set; }

    public virtual ICollection<ChiTietDonThuoc> ChiTietDonThuocs { get; set; } = new List<ChiTietDonThuoc>();

    public virtual PhieuKham PhieuKham { get; set; } = null!;
}
