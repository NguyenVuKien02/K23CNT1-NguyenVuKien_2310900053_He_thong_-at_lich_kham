namespace PhongKham.Business.DTOs.DanhMuc;

public class CaLamViecAdminDto
{
    public int Id { get; set; }

    public string TenCa { get; set; } = string.Empty;

    public TimeOnly GioBatDau { get; set; }

    public TimeOnly GioKetThuc { get; set; }

    public int ThoiLuongKhungGio { get; set; }

    public int SoKhungGio =>
        ThoiLuongKhungGio > 0 && GioKetThuc > GioBatDau
            ? (int)((GioKetThuc - GioBatDau).TotalMinutes / ThoiLuongKhungGio)
            : 0;

    public int SoLichLamViec { get; set; }
}
