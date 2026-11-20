namespace PhongKham.Business.DTOs.Doctors;

public class LichLamViecBacSiPublicDto
{
    public int Id { get; set; }

    public DateOnly NgayLam { get; set; }

    public string TenCa { get; set; } = string.Empty;

    public TimeOnly GioBatDau { get; set; }

    public TimeOnly GioKetThuc { get; set; }
}
