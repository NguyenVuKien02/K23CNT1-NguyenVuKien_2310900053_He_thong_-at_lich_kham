using PhongKham.Common.Helpers;

namespace PhongKham.Web.ViewModels.Admin;

public class NhomThuocAdminIndexViewModel
{
    public string? TuKhoa { get; set; }

    public PagedList<NhomThuocAdminItemViewModel> DanhSach { get; set; } = PagedList<NhomThuocAdminItemViewModel>.Empty();

    public bool CoBoLoc => !string.IsNullOrWhiteSpace(TuKhoa);
}
