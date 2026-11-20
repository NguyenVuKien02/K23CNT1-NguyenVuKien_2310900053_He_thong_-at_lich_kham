using PhongKham.Common.Helpers;

namespace PhongKham.Business.DTOs.DanhMuc;

public class ThuocFilterDto
{
    public string? TuKhoa { get; set; }

    public int? MedicineCategoryId { get; set; }

    public bool? TrangThai { get; set; }

    public string? TrangThaiTonKho { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = PagedList<ThuocAdminDto>.DefaultPageSize;
}
