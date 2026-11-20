using PhongKham.Common.Enums;
using PhongKham.Common.Helpers;

namespace PhongKham.Web.ViewModels.Admin;

public class NhanVienAdminItemViewModel
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public bool UserIsActive { get; set; }

    public string FullName { get; set; } = string.Empty;

    public GioiTinh? Gender { get; set; }

    public string? Phone { get; set; }

    public string? Position { get; set; }

    public DateOnly? HireDate { get; set; }

    public bool IsActive { get; set; }

    public string TenGioiTinh => EnumDisplayHelper.TenGioiTinh(Gender);
}
