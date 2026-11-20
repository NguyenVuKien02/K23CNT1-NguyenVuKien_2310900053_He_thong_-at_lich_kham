using System;
using System.Collections.Generic;
using PhongKham.Common.Constants;
using PhongKham.Common.Enums;
using PhongKham.Common.Results;

namespace PhongKham.Data.Entities;

public partial class LichHen
{
    public int Id { get; set; }

    public string MaLichHen { get; set; } = null!;

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public int LichLamViecId { get; set; }

    public DateOnly NgayHen { get; set; }

    public TimeOnly GioBatDau { get; set; }

    public TimeOnly GioKetThuc { get; set; }

    public string? LyDoKham { get; set; }

    public HinhThucDat HinhThucDat { get; set; }

    public TrangThaiLichHen TrangThai { get; set; }

    public string? LyDoHuy { get; set; }

    public int? NguoiTaoId { get; set; }

    public DateTime NgayTao { get; set; }

    public int? SoThuTu { get; set; }

    public virtual DanhGium? DanhGium { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual LichLamViec LichLamViec { get; set; } = null!;

    public virtual ICollection<LichSuLichHen> LichSuLichHens { get; set; } = new List<LichSuLichHen>();

    public virtual User? NguoiTao { get; set; }

    public virtual Patient Patient { get; set; } = null!;

    public virtual PhieuKham? PhieuKham { get; set; }

    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();

    public Result ChuyenTrangThai(TrangThaiLichHen moi)
    {
        var hopLe = (TrangThai, moi) switch
        {
            (TrangThaiLichHen.ChoXacNhan, TrangThaiLichHen.DaXacNhan) => true,
            (TrangThaiLichHen.ChoXacNhan, TrangThaiLichHen.DaHuy) => true,
            (TrangThaiLichHen.DaXacNhan, TrangThaiLichHen.DaCheckIn) => true,
            (TrangThaiLichHen.DaXacNhan, TrangThaiLichHen.DaHuy) => true,
            (TrangThaiLichHen.DaXacNhan, TrangThaiLichHen.VangMat) => true,
            (TrangThaiLichHen.DaCheckIn, TrangThaiLichHen.DangKham) => true,
            (TrangThaiLichHen.DaCheckIn, TrangThaiLichHen.DaHuy) => true,
            (TrangThaiLichHen.DangKham, TrangThaiLichHen.HoanThanh) => true,
            _ => false
        };

        if (!hopLe)
        {
            return Result.Fail($"Không thể chuyển trạng thái lịch hẹn từ \"{TenTrangThai(TrangThai)}\" sang \"{TenTrangThai(moi)}\".");
        }

        TrangThai = moi;
        return Result.Ok();
    }

    public bool CoTheHuy(DateTime bayGio, int gioHuyToiThieu = QuyDinh.GioHuyToiThieu)
    {
        if (TrangThai is not (TrangThaiLichHen.ChoXacNhan or TrangThaiLichHen.DaXacNhan))
        {
            return false;
        }

        var thoiGianHen = NgayHen.ToDateTime(GioBatDau);
        return thoiGianHen - bayGio >= TimeSpan.FromHours(gioHuyToiThieu);
    }

    private static string TenTrangThai(TrangThaiLichHen trangThai) => trangThai switch
    {
        TrangThaiLichHen.ChoXacNhan => "Chờ xác nhận",
        TrangThaiLichHen.DaXacNhan => "Đã xác nhận",
        TrangThaiLichHen.DaCheckIn => "Đã check-in",
        TrangThaiLichHen.DangKham => "Đang khám",
        TrangThaiLichHen.HoanThanh => "Hoàn thành",
        TrangThaiLichHen.DaHuy => "Đã hủy",
        TrangThaiLichHen.VangMat => "Vắng mặt",
        _ => trangThai.ToString()
    };
}
