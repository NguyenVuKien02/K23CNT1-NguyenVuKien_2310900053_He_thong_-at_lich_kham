using PhongKham.Common.Helpers;

namespace PhongKham.Web.ViewModels.Admin;

public class CaLamViecAdminIndexViewModel
{
    public string? TuKhoa { get; set; }

    public PagedList<CaLamViecAdminItemViewModel> DanhSach { get; set; } = PagedList<CaLamViecAdminItemViewModel>.Empty();

    public bool CoBoLoc => !string.IsNullOrWhiteSpace(TuKhoa);
}
