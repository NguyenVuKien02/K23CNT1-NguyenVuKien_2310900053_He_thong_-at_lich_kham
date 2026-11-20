using PhongKham.Common.Enums;

namespace PhongKham.Business.DTOs.Auth;

public class RegisterPatientDto
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public GioiTinh? Gender { get; set; }

    public string? Address { get; set; }
}
