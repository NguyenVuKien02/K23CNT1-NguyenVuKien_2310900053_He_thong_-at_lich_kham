namespace PhongKham.Web.ViewModels.Public;

public class ChuyenKhoaIndexViewModel
{
    public string? TuKhoa { get; set; }

    public IReadOnlyList<ChuyenKhoaCardViewModel> ChuyenKhoas { get; set; } = [];
}
