namespace PhongKham.Web.ViewModels.Public;

public class HomeIndexViewModel
{
    public IReadOnlyList<ChuyenKhoaCardViewModel> ChuyenKhoas { get; set; } = [];

    public IReadOnlyList<ChuyenKhoaCardViewModel> ChuyenKhoaNoiBat { get; set; } = [];

    public IReadOnlyList<BacSiCardViewModel> BacSiNoiBat { get; set; } = [];

    public IReadOnlyList<BacSiGoiYViewModel> GoiYBacSi { get; set; } = [];

    public int TongChuyenKhoa { get; set; }

    public int TongBacSi { get; set; }

    public int TongDichVu { get; set; }

    public DateOnly NgayMacDinh { get; set; } = DateOnly.FromDateTime(DateTime.Today);
}
