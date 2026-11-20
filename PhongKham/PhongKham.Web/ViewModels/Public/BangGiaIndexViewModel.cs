using PhongKham.Common.Enums;
using PhongKham.Common.Helpers;

namespace PhongKham.Web.ViewModels.Public;

public class BangGiaIndexViewModel
{
    public LoaiDichVu? LoaiDichVu { get; set; }

    public string? TuKhoa { get; set; }

    public PagedList<DichVuItemViewModel> DanhSachDichVu { get; set; } = PagedList<DichVuItemViewModel>.Empty();

    public bool CoBoLoc => LoaiDichVu.HasValue || !string.IsNullOrWhiteSpace(TuKhoa);
}
