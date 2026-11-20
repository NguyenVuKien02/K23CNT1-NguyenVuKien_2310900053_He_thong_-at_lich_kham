using PhongKham.Common.Enums;
using PhongKham.Common.Helpers;

namespace PhongKham.Web.ViewModels.Admin;

public class DichVuAdminIndexViewModel
{
    public string? TuKhoa { get; set; }

    public LoaiDichVu? LoaiDichVu { get; set; }

    public bool? ConHieuLuc { get; set; }

    public PagedList<DichVuAdminItemViewModel> DanhSach { get; set; } = PagedList<DichVuAdminItemViewModel>.Empty();

    public bool CoBoLoc =>
        !string.IsNullOrWhiteSpace(TuKhoa) ||
        LoaiDichVu.HasValue ||
        ConHieuLuc.HasValue;
}
