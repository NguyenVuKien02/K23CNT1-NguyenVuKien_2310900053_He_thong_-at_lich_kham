using PhongKham.Common.Helpers;

namespace PhongKham.Business.DTOs.TaiKhoan;

public class TaiKhoanFilterDto
{
    public string? TuKhoa { get; set; }

    public string? RoleName { get; set; }

    public bool? TrangThai { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = PagedList<TaiKhoanAdminDto>.DefaultPageSize;
}
