using PhongKham.Common.Helpers;

namespace PhongKham.Web.ViewModels.Admin;

public class TaiKhoanAdminItemViewModel
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string RoleName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public string TenVaiTro => EnumDisplayHelper.TenVaiTro(RoleName);
}
