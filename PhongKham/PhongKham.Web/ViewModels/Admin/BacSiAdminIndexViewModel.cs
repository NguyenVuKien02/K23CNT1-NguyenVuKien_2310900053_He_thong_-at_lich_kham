using Microsoft.AspNetCore.Mvc.Rendering;
using PhongKham.Common.Helpers;

namespace PhongKham.Web.ViewModels.Admin;

public class BacSiAdminIndexViewModel
{
    public string? TuKhoa { get; set; }

    public int? SpecialtyId { get; set; }

    public bool? TrangThai { get; set; }

    public List<SelectListItem> ChuyenKhoaOptions { get; set; } = new();

    public PagedList<BacSiAdminItemViewModel> DanhSach { get; set; } = PagedList<BacSiAdminItemViewModel>.Empty();

    public bool CoBoLoc =>
        !string.IsNullOrWhiteSpace(TuKhoa) ||
        SpecialtyId.HasValue ||
        TrangThai.HasValue;
}
