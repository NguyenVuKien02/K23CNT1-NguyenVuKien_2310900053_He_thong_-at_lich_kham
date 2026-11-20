namespace PhongKham.Business.DTOs.Auth;

public class AuthUserDto
{
    public int UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string RoleName { get; set; } = string.Empty;
}
