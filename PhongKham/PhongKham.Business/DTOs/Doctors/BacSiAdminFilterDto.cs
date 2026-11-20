using PhongKham.Common.Helpers;

namespace PhongKham.Business.DTOs.Doctors;

public class BacSiAdminFilterDto
{
    public string? TuKhoa { get; set; }

    public int? SpecialtyId { get; set; }

    public bool? TrangThai { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = PagedList<BacSiAdminDto>.DefaultPageSize;
}
