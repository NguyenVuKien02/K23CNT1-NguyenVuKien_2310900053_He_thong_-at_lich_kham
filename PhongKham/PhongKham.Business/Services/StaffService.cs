using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhongKham.Business.DTOs.Staffs;
using PhongKham.Business.Helpers;
using PhongKham.Common.Constants;
using PhongKham.Common.Enums;
using PhongKham.Common.Helpers;
using PhongKham.Common.Results;
using PhongKham.Data.Entities;
using PhongKham.Data.Repositories;

namespace PhongKham.Business.Services;

public class StaffService : IStaffService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;

    public StaffService(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<PagedList<NhanVienAdminDto>> TimKiemNhanVienAdminAsync(
        NhanVienFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        filter ??= new NhanVienFilterDto();
        var normalizedPageSize = filter.PageSize < 1 ? PagedList<NhanVienAdminDto>.DefaultPageSize : filter.PageSize;
        var normalizedPage = filter.Page < 1 ? 1 : filter.Page;

        var query = _unitOfWork.Repository<Staff>().Query()
            .AsNoTracking()
            .Where(s => s.User.Role.Name == RoleNames.Receptionist);

        if (filter.TrangThai.HasValue)
        {
            query = query.Where(s => s.IsActive == filter.TrangThai.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
        {
            var keyword = filter.TuKhoa.Trim();
            query = query.Where(s =>
                s.FullName.Contains(keyword) ||
                s.User.Email.Contains(keyword) ||
                (s.Phone != null && s.Phone.Contains(keyword)) ||
                (s.Position != null && s.Position.Contains(keyword)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return PagedList<NhanVienAdminDto>.Empty(normalizedPageSize);
        }

        var totalPages = (int)Math.Ceiling(totalCount / (double)normalizedPageSize);
        if (normalizedPage > totalPages)
        {
            normalizedPage = totalPages;
        }

        var items = await query
            .OrderByDescending(s => s.IsActive)
            .ThenBy(s => s.FullName)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(s => new NhanVienAdminDto
            {
                Id = s.Id,
                UserId = s.UserId,
                Email = s.User.Email,
                UserIsActive = s.User.IsActive,
                FullName = s.FullName,
                Gender = s.Gender,
                Phone = s.Phone,
                Position = s.Position,
                HireDate = s.HireDate,
                IsActive = s.IsActive
            })
            .ToListAsync(cancellationToken);

        return new PagedList<NhanVienAdminDto>(items, totalCount, normalizedPage, normalizedPageSize);
    }

    public async Task<Result<NhanVienAdminDto>> LayNhanVienAdminTheoIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<NhanVienAdminDto>.Fail("Không tìm thấy thông tin nhân viên.");
        }

        var item = await _unitOfWork.Repository<Staff>().Query()
            .AsNoTracking()
            .Where(s => s.Id == id && s.User.Role.Name == RoleNames.Receptionist)
            .Select(s => new NhanVienAdminDto
            {
                Id = s.Id,
                UserId = s.UserId,
                Email = s.User.Email,
                UserIsActive = s.User.IsActive,
                FullName = s.FullName,
                Gender = s.Gender,
                Phone = s.Phone,
                Position = s.Position,
                HireDate = s.HireDate,
                IsActive = s.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);

        return item is null
            ? Result<NhanVienAdminDto>.Fail("Không tìm thấy thông tin nhân viên.")
            : Result<NhanVienAdminDto>.Ok(item);
    }

    public async Task<Result<int>> TaoNhanVienAsync(
        NhanVienCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = BusinessValidationHelper.NormalizeEmail(dto.Email);
        if (!BusinessValidationHelper.IsValidEmail(normalizedEmail))
        {
            return Result<int>.Fail("Email không hợp lệ.");
        }

        if (normalizedEmail.Length > 150)
        {
            return Result<int>.Fail("Email không được vượt quá 150 ký tự.");
        }

        var passwordValidation = BusinessValidationHelper.KiemTraDoManhMatKhau(dto.Password);
        if (!passwordValidation.Success)
        {
            return Result<int>.Fail(passwordValidation.Error!);
        }

        var profileValidation = KiemTraHoSoNhanVien(
            dto.FullName,
            dto.Gender,
            dto.Phone,
            dto.Position,
            out var fullName,
            out var phone,
            out var position);

        if (!profileValidation.Success)
        {
            return Result<int>.Fail(profileValidation.Error!);
        }

        var userRepo = _unitOfWork.Repository<User>();
        var emailExists = await userRepo.Query()
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);
        if (emailExists)
        {
            return Result<int>.Fail("Email này đã được sử dụng trong hệ thống.");
        }

        var receptionistRole = await _unitOfWork.Repository<Role>().Query()
            .FirstOrDefaultAsync(r => r.Name == RoleNames.Receptionist, cancellationToken);
        if (receptionistRole is null)
        {
            return Result<int>.Fail("Không tìm thấy vai trò Lễ tân trong hệ thống.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = new User
            {
                RoleId = receptionistRole.Id,
                Email = normalizedEmail,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            var staff = new Staff
            {
                User = user,
                FullName = fullName,
                Gender = dto.Gender,
                Phone = phone,
                Position = position ?? "Lễ tân",
                HireDate = dto.HireDate,
                IsActive = dto.IsActive
            };

            await userRepo.AddAsync(user, cancellationToken);
            await _unitOfWork.Repository<Staff>().AddAsync(staff, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<int>.Ok(staff.Id);
        }
        catch (DbUpdateException ex) when (BusinessValidationHelper.IsUniqueConstraintViolation(ex))
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<int>.Fail("Email này đã được sử dụng trong hệ thống.");
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Result> CapNhatNhanVienAsync(
        NhanVienUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Id <= 0)
        {
            return Result.Fail("Không tìm thấy thông tin nhân viên.");
        }

        var profileValidation = KiemTraHoSoNhanVien(
            dto.FullName,
            dto.Gender,
            dto.Phone,
            dto.Position,
            out var fullName,
            out var phone,
            out var position);

        if (!profileValidation.Success)
        {
            return profileValidation;
        }

        var staffRepo = _unitOfWork.Repository<Staff>();
        var staff = await staffRepo.Query()
            .Include(s => s.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(s => s.Id == dto.Id && s.User.Role.Name == RoleNames.Receptionist, cancellationToken);
        if (staff is null)
        {
            return Result.Fail("Không tìm thấy thông tin nhân viên.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var statusChanged = staff.IsActive != dto.IsActive;
            staff.FullName = fullName;
            staff.Gender = dto.Gender;
            staff.Phone = phone;
            staff.Position = position ?? "Lễ tân";
            staff.HireDate = dto.HireDate;
            staff.IsActive = dto.IsActive;

            if (statusChanged && staff.User is not null)
            {
                staff.User.IsActive = dto.IsActive;
                _unitOfWork.Repository<User>().Update(staff.User);
            }

            staffRepo.Update(staff);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return Result.Ok();
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Result> DoiTrangThaiNhanVienAsync(
        int id,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result.Fail("Không tìm thấy thông tin nhân viên.");
        }

        var staffRepo = _unitOfWork.Repository<Staff>();
        var staff = await staffRepo.Query()
            .Include(s => s.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(s => s.Id == id && s.User.Role.Name == RoleNames.Receptionist, cancellationToken);
        if (staff is null)
        {
            return Result.Fail("Không tìm thấy thông tin nhân viên.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            staff.IsActive = isActive;
            if (staff.User is not null)
            {
                staff.User.IsActive = isActive;
                _unitOfWork.Repository<User>().Update(staff.User);
            }

            staffRepo.Update(staff);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return Result.Ok();
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static Result KiemTraHoSoNhanVien(
        string? rawFullName,
        GioiTinh? gender,
        string? rawPhone,
        string? rawPosition,
        out string fullName,
        out string? phone,
        out string? position)
    {
        fullName = (rawFullName ?? string.Empty).Trim();
        phone = BusinessValidationHelper.TrimOrNull(rawPhone);
        position = BusinessValidationHelper.TrimOrNull(rawPosition);

        if (string.IsNullOrWhiteSpace(fullName))
        {
            return Result.Fail("Vui lòng nhập họ và tên nhân viên.");
        }

        if (fullName.Length > 100)
        {
            return Result.Fail("Họ và tên không được vượt quá 100 ký tự.");
        }

        if (gender.HasValue && !Enum.IsDefined(gender.Value))
        {
            return Result.Fail("Giới tính không hợp lệ.");
        }

        if (phone is not null && phone.Length > 15)
        {
            return Result.Fail("Số điện thoại không được vượt quá 15 ký tự.");
        }

        if (position is not null && position.Length > 100)
        {
            return Result.Fail("Chức vụ không được vượt quá 100 ký tự.");
        }

        return Result.Ok();
    }
}
