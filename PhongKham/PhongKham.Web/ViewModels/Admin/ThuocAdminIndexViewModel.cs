using Microsoft.AspNetCore.Mvc.Rendering;
using PhongKham.Common.Helpers;

namespace PhongKham.Web.ViewModels.Admin;

public class ThuocAdminIndexViewModel
{
    public string? TuKhoa { get; set; }

    public int? MedicineCategoryId { get; set; }

    public bool? TrangThai { get; set; }

    public string? TrangThaiTonKho { get; set; }

    public List<SelectListItem> NhomThuocOptions { get; set; } = new();

    public PagedList<ThuocAdminItemViewModel> DanhSach { get; set; } = PagedList<ThuocAdminItemViewModel>.Empty();

    public bool CoBoLoc =>
        !string.IsNullOrWhiteSpace(TuKhoa) ||
        MedicineCategoryId.HasValue ||
        TrangThai.HasValue ||
        !string.IsNullOrWhiteSpace(TrangThaiTonKho);
}
