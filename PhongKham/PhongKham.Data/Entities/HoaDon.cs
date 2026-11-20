using System;
using System.Collections.Generic;
using PhongKham.Common.Enums;

namespace PhongKham.Data.Entities;

public partial class HoaDon
{
    public int Id { get; set; }

    public string MaHoaDon { get; set; } = null!;

    public int PhieuKhamId { get; set; }

    public int PatientId { get; set; }

    public int? NguoiLapId { get; set; }

    public decimal TongTien { get; set; }

    public decimal GiamGia { get; set; }

    public decimal? ThanhTien { get; set; }

    public PhuongThucThanhToan? PhuongThucThanhToan { get; set; }

    public TrangThaiHoaDon TrangThai { get; set; }

    public DateTime NgayLap { get; set; }

    public DateTime? NgayThanhToan { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual Staff? NguoiLap { get; set; }

    public virtual Patient Patient { get; set; } = null!;

    public virtual PhieuKham PhieuKham { get; set; } = null!;
}
