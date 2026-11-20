using PhongKham.Common.Helpers;

namespace PhongKham.Web.ViewModels.Admin;

public class NhanVienAdminIndexViewModel
{
    public string? TuKhoa { get; set; }

    public bool? TrangThai { get; set; }

    public PagedList<NhanVienAdminItemViewModel> DanhSach { get; set; } = PagedList<NhanVienAdminItemViewModel>.Empty();

    public bool CoBoLoc => !string.IsNullOrWhiteSpace(TuKhoa) || TrangThai.HasValue;
}
