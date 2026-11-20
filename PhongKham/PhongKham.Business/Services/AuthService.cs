using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PhongKham.Business.DTOs.Auth;
using PhongKham.Common.Constants;
using PhongKham.Common.Results;
using PhongKham.Data.Entities;
using PhongKham.Data.Repositories;

namespace PhongKham.Business.Services;

public class AuthService : IAuthService
{
    private static readonly EmailAddressAttribute EmailValidator = new();

    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILoginLockoutService _lockoutService;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        ILoginLockoutService lockoutService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _lockoutService = lockoutService;
    }

    public async Task<Result<AuthUserDto>> DangKyBenhNhanAsync(
        RegisterPatientDto dto,
        CancellationToken cancellationToken = default)
    {
        var fullName = dto.FullName?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return Result<AuthUserDto>.Fail("Vui lòng nhập họ và tên.");
        }

        if (fullName.Length > 100)
        {
            return Result<AuthUserDto>.Fail("Họ và tên không được vượt quá 100 ký tự.");
        }

        var normalizedEmail = NormalizeEmail(dto.Email);
        if (string.IsNullOrWhiteSpace(normalizedEmail) || !EmailValidator.IsValid(normalizedEmail))
        {
            return Result<AuthUserDto>.Fail("Email không hợp lệ.");
        }

        if (normalizedEmail.Length > 150)
        {
            return Result<AuthUserDto>.Fail("Email không được vượt quá 150 ký tự.");
        }

        var passwordValidation = KiemTraDoManhMatKhau(dto.Password);
        if (!passwordValidation.Success)
        {
            return Result<AuthUserDto>.Fail(passwordValidation.Error!);
        }

        var phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
        if (phone is not null && phone.Length > 15)
        {
            return Result<AuthUserDto>.Fail("Số điện thoại không được vượt quá 15 ký tự.");
        }

        var address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim();
        if (address is not null && address.Length > 300)
        {
            return Result<AuthUserDto>.Fail("Địa chỉ không được vượt quá 300 ký tự.");
        }

        if (dto.DateOfBirth.HasValue && dto.DateOfBirth.Value > DateOnly.FromDateTime(DateTime.Today))
        {
            return Result<AuthUserDto>.Fail("Ngày sinh không được lớn hơn ngày hiện tại.");
        }

        var userRepo = _unitOfWork.Repository<User>();
        var emailExists = await userRepo.Query()
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (emailExists)
        {
            return Result<AuthUserDto>.Fail("Email này đã được đăng ký. Vui lòng sử dụng email khác.");
        }

        var patientRole = await _unitOfWork.Repository<Role>().Query()
            .FirstOrDefaultAsync(r => r.Name == RoleNames.Patient, cancellationToken);

        if (patientRole is null)
        {
            return Result<AuthUserDto>.Fail("Không tìm thấy vai trò Bệnh nhân trong hệ thống.");
        }

        var now = DateTime.Now;
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = new User
            {
                RoleId = patientRole.Id,
                Email = normalizedEmail,
                IsActive = true,
                CreatedAt = now
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            var patient = new Patient
            {
                User = user,
                PatientCode = TaoMaTam("BN"),
                FullName = fullName,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                Phone = phone,
                Email = normalizedEmail,
                Address = address,
                CreatedAt = now
            };

            var hoSoBenhAn = new HoSoBenhAn
            {
                Patient = patient,
                MaHoSo = TaoMaTam("HS"),
                NgayTao = now
            };

            await userRepo.AddAsync(user, cancellationToken);
            await _unitOfWork.Repository<Patient>().AddAsync(patient, cancellationToken);
            await _unitOfWork.Repository<HoSoBenhAn>().AddAsync(hoSoBenhAn, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            patient.PatientCode = $"BN{patient.Id:D6}";
            hoSoBenhAn.MaHoSo = $"HS{hoSoBenhAn.Id:D6}";

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<AuthUserDto>.Ok(new AuthUserDto
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = patient.FullName,
                RoleName = patientRole.Name
            });
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<AuthUserDto>.Fail("Email này đã được đăng ký. Vui lòng sử dụng email khác.");
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Result<AuthUserDto>> DangNhapAsync(
        LoginDto dto,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(dto.Email);
        if (string.IsNullOrWhiteSpace(normalizedEmail) || string.IsNullOrEmpty(dto.Password))
        {
            return Result<AuthUserDto>.Fail("Vui lòng nhập đầy đủ email và mật khẩu.");
        }

        if (_lockoutService.IsLockedOut(normalizedEmail, out var remainingMinutes))
        {
            return Result<AuthUserDto>.Fail(TaoThongBaoKhoaTam(remainingMinutes));
        }

        var userRepo = _unitOfWork.Repository<User>();
        var user = await userRepo.Query()
            .Include(u => u.Role)
            .Include(u => u.Patient)
            .Include(u => u.Doctor)
            .Include(u => u.Staff)
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (user is null)
        {
            _lockoutService.RecordFailedAttempt(normalizedEmail);
            if (_lockoutService.IsLockedOut(normalizedEmail, out var minutesAfterFail))
            {
                return Result<AuthUserDto>.Fail(TaoThongBaoKhoaTam(minutesAfterFail));
            }

            return Result<AuthUserDto>.Fail("Email hoặc mật khẩu không đúng.");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            _lockoutService.RecordFailedAttempt(normalizedEmail);
            if (_lockoutService.IsLockedOut(normalizedEmail, out var minutesAfterFail))
            {
                return Result<AuthUserDto>.Fail(TaoThongBaoKhoaTam(minutesAfterFail));
            }

            return Result<AuthUserDto>.Fail("Email hoặc mật khẩu không đúng.");
        }

        if (!user.IsActive)
        {
            return Result<AuthUserDto>.Fail("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ phòng khám để được hỗ trợ.");
        }

        _lockoutService.ResetAttempts(normalizedEmail);

        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
        }

        user.LastLoginAt = DateTime.Now;
        userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var displayName = user.Patient?.FullName
            ?? user.Doctor?.FullName
            ?? user.Staff?.FullName
            ?? user.Email;

        return Result<AuthUserDto>.Ok(new AuthUserDto
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = displayName,
            RoleName = user.Role.Name
        });
    }

    public async Task<Result> DoiMatKhauAsync(
        ChangePasswordDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.UserId <= 0)
        {
            return Result.Fail("Không xác định được tài khoản.");
        }

        if (string.IsNullOrEmpty(dto.CurrentPassword))
        {
            return Result.Fail("Vui lòng nhập mật khẩu hiện tại.");
        }

        var passwordValidation = KiemTraDoManhMatKhau(dto.NewPassword);
        if (!passwordValidation.Success)
        {
            return passwordValidation;
        }

        if (string.Equals(dto.CurrentPassword, dto.NewPassword, StringComparison.Ordinal))
        {
            return Result.Fail("Mật khẩu mới phải khác mật khẩu hiện tại.");
        }

        var userRepo = _unitOfWork.Repository<User>();
        var user = await userRepo.GetByIdAsync(dto.UserId, cancellationToken);

        if (user is null || !user.IsActive)
        {
            return Result.Fail("Tài khoản không tồn tại hoặc đã bị khóa.");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.CurrentPassword);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return Result.Fail("Mật khẩu hiện tại không đúng.");
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
        userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }

    private static Result KiemTraDoManhMatKhau(string? password)
    {
        if (string.IsNullOrEmpty(password) ||
            password.Length < QuyDinh.DoDaiMatKhauToiThieu ||
            !password.Any(char.IsLetter) ||
            !password.Any(char.IsDigit))
        {
            return Result.Fail($"Mật khẩu phải có ít nhất {QuyDinh.DoDaiMatKhauToiThieu} ký tự, bao gồm cả chữ cái và chữ số.");
        }

        return Result.Ok();
    }

    private static string NormalizeEmail(string? email) =>
        (email ?? string.Empty).Trim().ToLowerInvariant();

    private static string TaoMaTam(string prefix) =>
        $"T{prefix}{Guid.NewGuid():N}"[..20];

    private static string TaoThongBaoKhoaTam(int remainingMinutes) =>
        $"Tài khoản tạm thời bị khóa do đăng nhập sai quá {QuyDinh.SoLanDangNhapSaiToiDa} lần trong {QuyDinh.ThoiGianKhoaDangNhapPhut} phút. Vui lòng thử lại sau {remainingMinutes} phút.";

    private static bool IsUniqueConstraintViolation(DbUpdateException exception) =>
        exception.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627);
}
