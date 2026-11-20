using PhongKham.Common.Enums;

namespace PhongKham.Business.DTOs.Staffs;

public class NhanVienUpdateDto
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public GioiTinh? Gender { get; set; }

    public string? Phone { get; set; }

    public string? Position { get; set; }

    public DateOnly? HireDate { get; set; }

    public bool IsActive { get; set; }
}
