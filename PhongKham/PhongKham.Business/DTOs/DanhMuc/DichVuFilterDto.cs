using PhongKham.Common.Enums;
using PhongKham.Common.Helpers;

namespace PhongKham.Business.DTOs.DanhMuc;

public class DichVuFilterDto
{
    public string? TuKhoa { get; set; }

    public LoaiDichVu? LoaiDichVu { get; set; }

    public bool? ConHieuLuc { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = PagedList<DichVuAdminDto>.DefaultPageSize;
}
