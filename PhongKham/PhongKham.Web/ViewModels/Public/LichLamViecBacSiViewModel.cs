namespace PhongKham.Web.ViewModels.Public;

public class LichLamViecBacSiViewModel
{
    public DateOnly NgayLam { get; set; }

    public string TenCa { get; set; } = string.Empty;

    public TimeOnly GioBatDau { get; set; }

    public TimeOnly GioKetThuc { get; set; }
}
