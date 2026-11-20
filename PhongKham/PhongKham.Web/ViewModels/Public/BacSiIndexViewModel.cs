using PhongKham.Common.Helpers;

namespace PhongKham.Web.ViewModels.Public;

public class BacSiIndexViewModel
{
    public int? ChuyenKhoaId { get; set; }

    public string? TuKhoa { get; set; }

    public DateOnly? NgayKham { get; set; }

    public string? TenChuyenKhoaDangLoc { get; set; }

    public IReadOnlyList<ChuyenKhoaCardViewModel> ChuyenKhoas { get; set; } = [];

    public PagedList<BacSiCardViewModel> DanhSachBacSi { get; set; } = PagedList<BacSiCardViewModel>.Empty();

    public bool CoBoLoc =>
        ChuyenKhoaId.HasValue ||
        !string.IsNullOrWhiteSpace(TuKhoa) ||
        NgayKham.HasValue;
}
