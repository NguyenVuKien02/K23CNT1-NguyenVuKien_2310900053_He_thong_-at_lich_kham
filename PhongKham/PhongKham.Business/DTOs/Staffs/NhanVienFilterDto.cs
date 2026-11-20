using PhongKham.Common.Helpers;

namespace PhongKham.Business.DTOs.Staffs;

public class NhanVienFilterDto
{
    public string? TuKhoa { get; set; }

    public bool? TrangThai { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = PagedList<NhanVienAdminDto>.DefaultPageSize;
}
