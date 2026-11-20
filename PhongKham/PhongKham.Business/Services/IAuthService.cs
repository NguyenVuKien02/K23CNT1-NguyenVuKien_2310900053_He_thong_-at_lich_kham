using PhongKham.Business.DTOs.Auth;
using PhongKham.Common.Results;

namespace PhongKham.Business.Services;

public interface IAuthService
{
    Task<Result<AuthUserDto>> DangKyBenhNhanAsync(RegisterPatientDto dto, CancellationToken cancellationToken = default);

    Task<Result<AuthUserDto>> DangNhapAsync(LoginDto dto, CancellationToken cancellationToken = default);

    Task<Result> DoiMatKhauAsync(ChangePasswordDto dto, CancellationToken cancellationToken = default);
}
