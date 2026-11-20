using PhongKham.Common.Helpers;

namespace PhongKham.Web.ViewModels.Admin;

public class ChuyenKhoaAdminIndexViewModel
{
    public string? TuKhoa { get; set; }

    public bool? TrangThai { get; set; }

    public PagedList<ChuyenKhoaAdminItemViewModel> DanhSach { get; set; } = PagedList<ChuyenKhoaAdminItemViewModel>.Empty();

    public bool CoBoLoc => !string.IsNullOrWhiteSpace(TuKhoa) || TrangThai.HasValue;
}
