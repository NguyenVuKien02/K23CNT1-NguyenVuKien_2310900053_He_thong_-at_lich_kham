using PhongKham.Common.Enums;
using PhongKham.Common.Helpers;

namespace PhongKham.Web.ViewModels.Admin;

public class BacSiAdminItemViewModel
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public bool UserIsActive { get; set; }

    public int SpecialtyId { get; set; }

    public string SpecialtyName { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public GioiTinh? Gender { get; set; }

    public string? Phone { get; set; }

    public string? Degree { get; set; }

    public int? ExperienceYears { get; set; }

    public decimal ConsultationFee { get; set; }

    public bool IsActive { get; set; }

    public string TenGioiTinh => EnumDisplayHelper.TenGioiTinh(Gender);
}
