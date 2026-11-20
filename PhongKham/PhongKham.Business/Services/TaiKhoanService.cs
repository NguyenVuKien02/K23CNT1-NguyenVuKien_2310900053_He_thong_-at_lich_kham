using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhongKham.Business.DTOs.TaiKhoan;
using PhongKham.Business.Helpers;
using PhongKham.Common.Helpers;
using PhongKham.Common.Results;
using PhongKham.Data.Entities;
using PhongKham.Data.Repositories;

namespace PhongKham.Business.Services;

public class TaiKhoanService : ITaiKhoanService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILoginLockoutService _lockoutService;

    public TaiKhoanService(
        IUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        ILoginLockoutService lockoutService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _lockoutService = lockoutService;
    }

    public async Task<PagedList<TaiKhoanAdminDto>> TimKiemTaiKhoanAdminAsync(
        TaiKhoanFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        filter ??= new TaiKhoanFilterDto();
        var normalizedPageSize = filter.PageSize < 1 ? PagedList<TaiKhoanAdminDto>.DefaultPageSize : filter.PageSize;
        var normalizedPage = filter.Page < 1 ? 1 : filter.Page;

        var query = _unitOfWork.Repository<User>().Query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.RoleName))
        {
            var roleName = filter.RoleName.Trim();
            query = query.Where(u => u.Role.Name == roleName);
        }

        if (filter.TrangThai.HasValue)
        {
            query = query.Where(u => u.IsActive == filter.TrangThai.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
        {
            var keyword = filter.TuKhoa.Trim();
            query = query.Where(u =>
                u.Email.Contains(keyword) ||
                (u.Doctor != null && u.Doctor.FullName.Contains(keyword)) ||
                (u.Staff != null && u.Staff.FullName.Contains(keyword)) ||
                (u.Patient != null && u.Patient.FullName.Contains(keyword)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return PagedList<TaiKhoanAdminDto>.Empty(normalizedPageSize);
        }

        var totalPages = (int)Math.Ceiling(totalCount / (double)normalizedPageSize);
        if (normalizedPage > totalPages)
        {
            normalizedPage = totalPages;
        }

        var rawItems = await query
            .OrderByDescending(u => u.CreatedAt)
            .ThenBy(u => u.Email)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(u => new
            {
                u.Id,
                u.Email,
                DoctorName = u.Doctor != null ? u.Doctor.FullName : null,
                StaffName = u.Staff != null ? u.Staff.FullName : null,
                PatientName = u.Patient != null ? u.Patient.FullName : null,
                RoleName = u.Role.Name,
                u.IsActive,
                u.CreatedAt,
                u.LastLoginAt
            })
            .ToListAsync(cancellationToken);

        var items = rawItems.Select(u => new TaiKhoanAdminDto
        {
            Id = u.Id,
            Email = u.Email,
            FullName = u.DoctorName ?? u.StaffName ?? u.PatientName ?? EnumDisplayHelper.TenVaiTro(u.RoleName),
            RoleName = u.RoleName,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            LastLoginAt = u.LastLoginAt
        }).ToList();

        return new PagedList<TaiKhoanAdminDto>(items, totalCount, normalizedPage, normalizedPageSize);
    }

    public async Task<Result<TaiKhoanAdminDto>> LayTaiKhoanTheoIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<TaiKhoanAdminDto>.Fail("Không tìm thấy tài khoản.");
        }

        var rawItem = await _unitOfWork.Repository<User>().Query()
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new
            {
                u.Id,
                u.Email,
                DoctorName = u.Doctor != null ? u.Doctor.FullName : null,
                StaffName = u.Staff != null ? u.Staff.FullName : null,
                PatientName = u.Patient != null ? u.Patient.FullName : null,
                RoleName = u.Role.Name,
                u.IsActive,
                u.CreatedAt,
                u.LastLoginAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (rawItem is null)
        {
            return Result<TaiKhoanAdminDto>.Fail("Không tìm thấy tài khoản.");
        }

        return Result<TaiKhoanAdminDto>.Ok(new TaiKhoanAdminDto
        {
            Id = rawItem.Id,
            Email = rawItem.Email,
            FullName = rawItem.DoctorName ?? rawItem.StaffName ?? rawItem.PatientName ?? EnumDisplayHelper.TenVaiTro(rawItem.RoleName),
            RoleName = rawItem.RoleName,
            IsActive = rawItem.IsActive,
            CreatedAt = rawItem.CreatedAt,
            LastLoginAt = rawItem.LastLoginAt
        });
    }

    public async Task<Result> DoiTrangThaiTaiKhoanAsync(
        int userId,
        bool isActive,
        int currentAdminUserId,
        CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
        {
            return Result.Fail("Không tìm thấy tài khoản.");
        }

        if (!isActive && userId == currentAdminUserId)
        {
            return Result.Fail("Bạn không thể tự khóa tài khoản đang đăng nhập.");
        }

        var userRepo = _unitOfWork.Repository<User>();
        var user = await userRepo.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result.Fail("Không tìm thấy tài khoản.");
        }

        user.IsActive = isActive;
        userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (isActive)
        {
            _lockoutService.ResetAttempts(BusinessValidationHelper.NormalizeEmail(user.Email));
        }

        return Result.Ok();
    }

    public async Task<Result> DatLaiMatKhauAsync(
        DatLaiMatKhauAdminDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.UserId <= 0)
        {
            return Result.Fail("Không tìm thấy tài khoản.");
        }

        var passwordValidation = BusinessValidationHelper.KiemTraDoManhMatKhau(dto.NewPassword);
        if (!passwordValidation.Success)
        {
            return passwordValidation;
        }

        var userRepo = _unitOfWork.Repository<User>();
        var user = await userRepo.GetByIdAsync(dto.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Fail("Không tìm thấy tài khoản.");
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
        userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _lockoutService.ResetAttempts(BusinessValidationHelper.NormalizeEmail(user.Email));

        return Result.Ok();
    }
}
