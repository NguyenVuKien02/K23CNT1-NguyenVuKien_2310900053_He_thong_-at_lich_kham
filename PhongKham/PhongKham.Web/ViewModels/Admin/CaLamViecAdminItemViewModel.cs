namespace PhongKham.Web.ViewModels.Admin;

public class CaLamViecAdminItemViewModel
{
    public int Id { get; set; }

    public string TenCa { get; set; } = string.Empty;

    public TimeOnly GioBatDau { get; set; }

    public TimeOnly GioKetThuc { get; set; }

    public int ThoiLuongKhungGio { get; set; }

    public int SoKhungGio { get; set; }

    public int SoLichLamViec { get; set; }
}
