using PhongKham.Common.Helpers;

namespace PhongKham.Web.ViewModels.Admin;

public class TaiKhoanAdminIndexViewModel
{
    public string? TuKhoa { get; set; }

    public string? RoleName { get; set; }

    public bool? TrangThai { get; set; }

    public int CurrentAdminUserId { get; set; }

    public PagedList<TaiKhoanAdminItemViewModel> DanhSach { get; set; } = PagedList<TaiKhoanAdminItemViewModel>.Empty();

    public bool CoBoLoc =>
        !string.IsNullOrWhiteSpace(TuKhoa) ||
        !string.IsNullOrWhiteSpace(RoleName) ||
        TrangThai.HasValue;
}
