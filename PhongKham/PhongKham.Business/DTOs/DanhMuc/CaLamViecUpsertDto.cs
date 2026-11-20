namespace PhongKham.Business.DTOs.DanhMuc;

public class CaLamViecUpsertDto
{
    public int Id { get; set; }

    public string TenCa { get; set; } = string.Empty;

    public TimeOnly GioBatDau { get; set; }

    public TimeOnly GioKetThuc { get; set; }

    public int ThoiLuongKhungGio { get; set; } = 30;
}
