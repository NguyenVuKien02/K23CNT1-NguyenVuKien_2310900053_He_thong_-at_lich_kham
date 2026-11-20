using PhongKham.Common.Enums;

namespace PhongKham.Business.DTOs.Doctors;

public class BacSiUpdateDto
{
    public int Id { get; set; }

    public int SpecialtyId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public GioiTinh? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Phone { get; set; }

    public string? Degree { get; set; }

    public int? ExperienceYears { get; set; }

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    public decimal ConsultationFee { get; set; }

    public bool IsActive { get; set; }
}
