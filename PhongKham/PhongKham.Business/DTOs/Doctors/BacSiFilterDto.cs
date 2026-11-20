using PhongKham.Common.Helpers;

namespace PhongKham.Business.DTOs.Doctors;

public class BacSiFilterDto
{
    public int? ChuyenKhoaId { get; set; }

    public string? TuKhoa { get; set; }

    public DateOnly? NgayKham { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = PagedList<BacSiPublicDto>.DefaultPageSize;
}
