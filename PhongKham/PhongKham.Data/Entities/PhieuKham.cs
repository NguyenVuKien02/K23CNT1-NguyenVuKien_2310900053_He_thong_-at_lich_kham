using System;
using System.Collections.Generic;
using PhongKham.Common.Enums;

namespace PhongKham.Data.Entities;

public partial class PhieuKham
{
    public int Id { get; set; }

    public string MaPhieuKham { get; set; } = null!;

    public int HoSoBenhAnId { get; set; }

    public int? LichHenId { get; set; }

    public int DoctorId { get; set; }

    public DateTime NgayKham { get; set; }

    public string? TrieuChung { get; set; }

    public decimal? NhietDo { get; set; }

    public int? Mach { get; set; }

    public int? HuyetApTamThu { get; set; }

    public int? HuyetApTamTruong { get; set; }

    public decimal? CanNang { get; set; }

    public decimal? ChieuCao { get; set; }

    public string? ChanDoan { get; set; }

    public string? KetLuan { get; set; }

    public string? LoiDanBacSi { get; set; }

    public DateOnly? NgayTaiKham { get; set; }

    public TrangThaiPhieuKham TrangThai { get; set; }

    public virtual ICollection<ChiDinhDichVu> ChiDinhDichVus { get; set; } = new List<ChiDinhDichVu>();

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual ICollection<DonThuoc> DonThuocs { get; set; } = new List<DonThuoc>();

    public virtual HoSoBenhAn HoSoBenhAn { get; set; } = null!;

    public virtual HoaDon? HoaDon { get; set; }

    public virtual LichHen? LichHen { get; set; }
}
