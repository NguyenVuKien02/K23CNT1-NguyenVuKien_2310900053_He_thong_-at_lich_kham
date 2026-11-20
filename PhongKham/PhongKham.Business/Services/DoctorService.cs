using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhongKham.Business.DTOs.Doctors;
using PhongKham.Business.Helpers;
using PhongKham.Common.Constants;
using PhongKham.Common.Enums;
using PhongKham.Common.Helpers;
using PhongKham.Common.Results;
using PhongKham.Data.Entities;
using PhongKham.Data.Repositories;

namespace PhongKham.Business.Services;

public class DoctorService : IDoctorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;

    public DoctorService(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    #region Public

    public async Task<IReadOnlyList<BacSiPublicDto>> LayBacSiNoiBatAsync(
        int soLuong = 6,
        CancellationToken cancellationToken = default)
    {
        var takeCount = Math.Clamp(soLuong, 1, 20);

        var items = await BuildActiveDoctorQuery()
            .Select(d => new BacSiPublicDto
            {
                Id = d.Id,
                SpecialtyId = d.SpecialtyId,
                SpecialtyName = d.Specialty.Name,
                FullName = d.FullName,
                Degree = d.Degree,
                ExperienceYears = d.ExperienceYears,
                Bio = d.Bio,
                AvatarUrl = d.AvatarUrl,
                ConsultationFee = d.ConsultationFee,
                DiemDanhGiaTrungBinh = d.DanhGia
                    .Where(dg => dg.HienThi)
                    .Average(dg => (double?)dg.SoSao) ?? 0.0,
                SoLuotDanhGia = d.DanhGia.Count(dg => dg.HienThi)
            })
            .OrderByDescending(d => d.DiemDanhGiaTrungBinh)
            .ThenByDescending(d => d.SoLuotDanhGia)
            .ThenByDescending(d => d.ExperienceYears ?? 0)
            .ThenBy(d => d.FullName)
            .Take(takeCount)
            .ToListAsync(cancellationToken);

        RoundAverageRatings(items);
        return items;
    }

    public async Task<PagedList<BacSiPublicDto>> TimKiemBacSiPublicAsync(
        BacSiFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        filter ??= new BacSiFilterDto();

        var normalizedPageSize = filter.PageSize < 1
            ? PagedList<BacSiPublicDto>.DefaultPageSize
            : filter.PageSize;
        var normalizedPage = filter.Page < 1 ? 1 : filter.Page;

        var query = BuildActiveDoctorQuery();

        if (filter.ChuyenKhoaId.HasValue && filter.ChuyenKhoaId.Value > 0)
        {
            var specialtyId = filter.ChuyenKhoaId.Value;
            query = query.Where(d => d.SpecialtyId == specialtyId);
        }

        if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
        {
            var keyword = filter.TuKhoa.Trim();
            query = query.Where(d =>
                d.FullName.Contains(keyword) ||
                (d.Degree != null && d.Degree.Contains(keyword)) ||
                d.Specialty.Name.Contains(keyword));
        }

        if (filter.NgayKham.HasValue)
        {
            var ngayKham = filter.NgayKham.Value;
            query = query.Where(d =>
                d.LichLamViecs.Any(llv => llv.ConHieuLuc && llv.NgayLam == ngayKham));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return PagedList<BacSiPublicDto>.Empty(normalizedPageSize);
        }

        var totalPages = (int)Math.Ceiling(totalCount / (double)normalizedPageSize);
        if (normalizedPage > totalPages)
        {
            normalizedPage = totalPages;
        }

        var items = await query
            .Select(d => new BacSiPublicDto
            {
                Id = d.Id,
                SpecialtyId = d.SpecialtyId,
                SpecialtyName = d.Specialty.Name,
                FullName = d.FullName,
                Degree = d.Degree,
                ExperienceYears = d.ExperienceYears,
                Bio = d.Bio,
                AvatarUrl = d.AvatarUrl,
                ConsultationFee = d.ConsultationFee,
                DiemDanhGiaTrungBinh = d.DanhGia
                    .Where(dg => dg.HienThi)
                    .Average(dg => (double?)dg.SoSao) ?? 0.0,
                SoLuotDanhGia = d.DanhGia.Count(dg => dg.HienThi)
            })
            .OrderByDescending(d => d.DiemDanhGiaTrungBinh)
            .ThenByDescending(d => d.SoLuotDanhGia)
            .ThenBy(d => d.FullName)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToListAsync(cancellationToken);

        RoundAverageRatings(items);
        return new PagedList<BacSiPublicDto>(items, totalCount, normalizedPage, normalizedPageSize);
    }

    public async Task<Result<BacSiChiTietDto>> LayChiTietBacSiPublicAsync(
        int id,
        int reviewPage = 1,
        int reviewPageSize = PagedList<DanhGiaBacSiPublicDto>.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<BacSiChiTietDto>.Fail("Không tìm thấy thông tin bác sĩ.");
        }

        var doctor = await BuildActiveDoctorQuery()
            .Where(d => d.Id == id)
            .Select(d => new BacSiChiTietDto
            {
                Id = d.Id,
                SpecialtyId = d.SpecialtyId,
                SpecialtyName = d.Specialty.Name,
                SpecialtyDescription = d.Specialty.Description,
                FullName = d.FullName,
                Gender = d.Gender,
                Degree = d.Degree,
                ExperienceYears = d.ExperienceYears,
                Bio = d.Bio,
                AvatarUrl = d.AvatarUrl,
                ConsultationFee = d.ConsultationFee
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (doctor is null)
        {
            return Result<BacSiChiTietDto>.Fail("Không tìm thấy thông tin bác sĩ.");
        }

        var ratingGroups = await _unitOfWork.Repository<DanhGium>().Query()
            .AsNoTracking()
            .Where(dg => dg.DoctorId == id && dg.HienThi)
            .GroupBy(dg => dg.SoSao)
            .Select(g => new { SoSao = (int)g.Key, SoLuong = g.Count() })
            .ToListAsync(cancellationToken);

        var phanBo = new Dictionary<int, int>
        {
            [5] = 0,
            [4] = 0,
            [3] = 0,
            [2] = 0,
            [1] = 0
        };

        var totalReviews = 0;
        var weightedSum = 0;
        foreach (var group in ratingGroups)
        {
            if (phanBo.ContainsKey(group.SoSao))
            {
                phanBo[group.SoSao] = group.SoLuong;
            }

            totalReviews += group.SoLuong;
            weightedSum += group.SoSao * group.SoLuong;
        }

        doctor.PhanBoSoSao = phanBo;
        doctor.SoLuotDanhGia = totalReviews;
        doctor.DiemDanhGiaTrungBinh = totalReviews == 0
            ? 0.0
            : Math.Round(weightedSum / (double)totalReviews, 1);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var endDate = today.AddDays(14);
        doctor.LichLamViecSapToi = await _unitOfWork.Repository<LichLamViec>().Query()
            .AsNoTracking()
            .Where(llv =>
                llv.DoctorId == id &&
                llv.ConHieuLuc &&
                llv.NgayLam >= today &&
                llv.NgayLam <= endDate)
            .OrderBy(llv => llv.NgayLam)
            .ThenBy(llv => llv.CaLamViec.GioBatDau)
            .Select(llv => new LichLamViecBacSiPublicDto
            {
                Id = llv.Id,
                NgayLam = llv.NgayLam,
                TenCa = llv.CaLamViec.TenCa,
                GioBatDau = llv.CaLamViec.GioBatDau,
                GioKetThuc = llv.CaLamViec.GioKetThuc
            })
            .ToListAsync(cancellationToken);

        var normalizedReviewPageSize = reviewPageSize < 1
            ? PagedList<DanhGiaBacSiPublicDto>.DefaultPageSize
            : reviewPageSize;
        var normalizedReviewPage = reviewPage < 1 ? 1 : reviewPage;

        if (totalReviews == 0)
        {
            doctor.DanhGia = PagedList<DanhGiaBacSiPublicDto>.Empty(normalizedReviewPageSize);
        }
        else
        {
            var totalReviewPages = (int)Math.Ceiling(totalReviews / (double)normalizedReviewPageSize);
            if (normalizedReviewPage > totalReviewPages)
            {
                normalizedReviewPage = totalReviewPages;
            }

            var reviewItems = await _unitOfWork.Repository<DanhGium>().Query()
                .AsNoTracking()
                .Where(dg => dg.DoctorId == id && dg.HienThi)
                .OrderByDescending(dg => dg.NgayDanhGia)
                .ThenByDescending(dg => dg.Id)
                .Skip((normalizedReviewPage - 1) * normalizedReviewPageSize)
                .Take(normalizedReviewPageSize)
                .Select(dg => new DanhGiaBacSiPublicDto
                {
                    Id = dg.Id,
                    TenBenhNhan = dg.Patient.FullName,
                    SoSao = dg.SoSao,
                    NhanXet = dg.NhanXet,
                    NgayDanhGia = dg.NgayDanhGia
                })
                .ToListAsync(cancellationToken);

            doctor.DanhGia = new PagedList<DanhGiaBacSiPublicDto>(
                reviewItems,
                totalReviews,
                normalizedReviewPage,
                normalizedReviewPageSize);
        }

        return Result<BacSiChiTietDto>.Ok(doctor);
    }

    public async Task<IReadOnlyList<BacSiGoiYDto>> LayDanhSachGoiYBacSiAsync(
        CancellationToken cancellationToken = default)
    {
        return await BuildActiveDoctorQuery()
            .OrderBy(d => d.FullName)
            .Select(d => new BacSiGoiYDto
            {
                Id = d.Id,
                SpecialtyId = d.SpecialtyId,
                FullName = d.FullName,
                Degree = d.Degree
            })
            .ToListAsync(cancellationToken);
    }

    public Task<int> DemBacSiHoatDongAsync(CancellationToken cancellationToken = default)
    {
        return BuildActiveDoctorQuery().CountAsync(cancellationToken);
    }

    #endregion

    #region Admin

    public async Task<PagedList<BacSiAdminDto>> TimKiemBacSiAdminAsync(
        BacSiAdminFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        filter ??= new BacSiAdminFilterDto();
        var normalizedPageSize = filter.PageSize < 1 ? PagedList<BacSiAdminDto>.DefaultPageSize : filter.PageSize;
        var normalizedPage = filter.Page < 1 ? 1 : filter.Page;

        var query = _unitOfWork.Repository<Doctor>().Query().AsNoTracking();

        if (filter.SpecialtyId.HasValue && filter.SpecialtyId.Value > 0)
        {
            var specialtyId = filter.SpecialtyId.Value;
            query = query.Where(d => d.SpecialtyId == specialtyId);
        }

        if (filter.TrangThai.HasValue)
        {
            query = query.Where(d => d.IsActive == filter.TrangThai.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
        {
            var keyword = filter.TuKhoa.Trim();
            query = query.Where(d =>
                d.FullName.Contains(keyword) ||
                d.User.Email.Contains(keyword) ||
                (d.Phone != null && d.Phone.Contains(keyword)) ||
                (d.Degree != null && d.Degree.Contains(keyword)) ||
                d.Specialty.Name.Contains(keyword));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return PagedList<BacSiAdminDto>.Empty(normalizedPageSize);
        }

        var totalPages = (int)Math.Ceiling(totalCount / (double)normalizedPageSize);
        if (normalizedPage > totalPages)
        {
            normalizedPage = totalPages;
        }

        var items = await query
            .OrderByDescending(d => d.IsActive)
            .ThenBy(d => d.FullName)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(d => new BacSiAdminDto
            {
                Id = d.Id,
                UserId = d.UserId,
                Email = d.User.Email,
                UserIsActive = d.User.IsActive,
                SpecialtyId = d.SpecialtyId,
                SpecialtyName = d.Specialty.Name,
                FullName = d.FullName,
                Gender = d.Gender,
                DateOfBirth = d.DateOfBirth,
                Phone = d.Phone,
                Degree = d.Degree,
                ExperienceYears = d.ExperienceYears,
                Bio = d.Bio,
                AvatarUrl = d.AvatarUrl,
                ConsultationFee = d.ConsultationFee,
                IsActive = d.IsActive
            })
            .ToListAsync(cancellationToken);

        return new PagedList<BacSiAdminDto>(items, totalCount, normalizedPage, normalizedPageSize);
    }

    public async Task<Result<BacSiAdminDto>> LayBacSiAdminTheoIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<BacSiAdminDto>.Fail("Không tìm thấy thông tin bác sĩ.");
        }

        var item = await _unitOfWork.Repository<Doctor>().Query()
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new BacSiAdminDto
            {
                Id = d.Id,
                UserId = d.UserId,
                Email = d.User.Email,
                UserIsActive = d.User.IsActive,
                SpecialtyId = d.SpecialtyId,
                SpecialtyName = d.Specialty.Name,
                FullName = d.FullName,
                Gender = d.Gender,
                DateOfBirth = d.DateOfBirth,
                Phone = d.Phone,
                Degree = d.Degree,
                ExperienceYears = d.ExperienceYears,
                Bio = d.Bio,
                AvatarUrl = d.AvatarUrl,
                ConsultationFee = d.ConsultationFee,
                IsActive = d.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);

        return item is null
            ? Result<BacSiAdminDto>.Fail("Không tìm thấy thông tin bác sĩ.")
            : Result<BacSiAdminDto>.Ok(item);
    }

    public async Task<Result<int>> TaoBacSiAsync(
        BacSiCreateDto dto,
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

        var profileValidation = KiemTraHoSoBacSi(
            dto.FullName,
            dto.SpecialtyId,
            dto.Gender,
            dto.DateOfBirth,
            dto.Phone,
            dto.Degree,
            dto.ExperienceYears,
            dto.Bio,
            dto.AvatarUrl,
            dto.ConsultationFee,
            out var fullName,
            out var phone,
            out var degree,
            out var bio,
            out var avatarUrl);

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

        var specialtyExists = await _unitOfWork.Repository<Specialty>().Query()
            .AnyAsync(s => s.Id == dto.SpecialtyId && s.IsActive, cancellationToken);
        if (!specialtyExists)
        {
            return Result<int>.Fail("Chuyên khoa đã chọn không tồn tại hoặc đang ngừng hoạt động.");
        }

        var doctorRole = await _unitOfWork.Repository<Role>().Query()
            .FirstOrDefaultAsync(r => r.Name == RoleNames.Doctor, cancellationToken);
        if (doctorRole is null)
        {
            return Result<int>.Fail("Không tìm thấy vai trò Bác sĩ trong hệ thống.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = new User
            {
                RoleId = doctorRole.Id,
                Email = normalizedEmail,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            var doctor = new Doctor
            {
                User = user,
                SpecialtyId = dto.SpecialtyId,
                FullName = fullName,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Phone = phone,
                Degree = degree,
                ExperienceYears = dto.ExperienceYears,
                Bio = bio,
                AvatarUrl = avatarUrl,
                ConsultationFee = Math.Truncate(dto.ConsultationFee),
                IsActive = dto.IsActive
            };

            await userRepo.AddAsync(user, cancellationToken);
            await _unitOfWork.Repository<Doctor>().AddAsync(doctor, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<int>.Ok(doctor.Id);
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

    public async Task<Result> CapNhatBacSiAsync(
        BacSiUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Id <= 0)
        {
            return Result.Fail("Không tìm thấy thông tin bác sĩ.");
        }

        var profileValidation = KiemTraHoSoBacSi(
            dto.FullName,
            dto.SpecialtyId,
            dto.Gender,
            dto.DateOfBirth,
            dto.Phone,
            dto.Degree,
            dto.ExperienceYears,
            dto.Bio,
            dto.AvatarUrl,
            dto.ConsultationFee,
            out var fullName,
            out var phone,
            out var degree,
            out var bio,
            out var avatarUrl);

        if (!profileValidation.Success)
        {
            return profileValidation;
        }

        var doctorRepo = _unitOfWork.Repository<Doctor>();
        var doctor = await doctorRepo.Query()
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == dto.Id, cancellationToken);
        if (doctor is null)
        {
            return Result.Fail("Không tìm thấy thông tin bác sĩ.");
        }

        var specialtyExists = await _unitOfWork.Repository<Specialty>().Query()
            .AnyAsync(s => s.Id == dto.SpecialtyId, cancellationToken);
        if (!specialtyExists)
        {
            return Result.Fail("Chuyên khoa đã chọn không tồn tại.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var statusChanged = doctor.IsActive != dto.IsActive;
            doctor.SpecialtyId = dto.SpecialtyId;
            doctor.FullName = fullName;
            doctor.Gender = dto.Gender;
            doctor.DateOfBirth = dto.DateOfBirth;
            doctor.Phone = phone;
            doctor.Degree = degree;
            doctor.ExperienceYears = dto.ExperienceYears;
            doctor.Bio = bio;
            doctor.AvatarUrl = avatarUrl;
            doctor.ConsultationFee = Math.Truncate(dto.ConsultationFee);
            doctor.IsActive = dto.IsActive;

            if (statusChanged && doctor.User is not null)
            {
                doctor.User.IsActive = dto.IsActive;
                _unitOfWork.Repository<User>().Update(doctor.User);
            }

            doctorRepo.Update(doctor);
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

    public async Task<Result> DoiTrangThaiBacSiAsync(
        int id,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result.Fail("Không tìm thấy thông tin bác sĩ.");
        }

        var doctorRepo = _unitOfWork.Repository<Doctor>();
        var doctor = await doctorRepo.Query()
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (doctor is null)
        {
            return Result.Fail("Không tìm thấy thông tin bác sĩ.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            doctor.IsActive = isActive;
            if (doctor.User is not null)
            {
                doctor.User.IsActive = isActive;
                _unitOfWork.Repository<User>().Update(doctor.User);
            }

            doctorRepo.Update(doctor);
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

    private static Result KiemTraHoSoBacSi(
        string? rawFullName,
        int specialtyId,
        GioiTinh? gender,
        DateOnly? dateOfBirth,
        string? rawPhone,
        string? rawDegree,
        int? experienceYears,
        string? rawBio,
        string? rawAvatarUrl,
        decimal consultationFee,
        out string fullName,
        out string? phone,
        out string? degree,
        out string? bio,
        out string? avatarUrl)
    {
        fullName = (rawFullName ?? string.Empty).Trim();
        phone = BusinessValidationHelper.TrimOrNull(rawPhone);
        degree = BusinessValidationHelper.TrimOrNull(rawDegree);
        bio = BusinessValidationHelper.TrimOrNull(rawBio);
        avatarUrl = BusinessValidationHelper.TrimOrNull(rawAvatarUrl);

        if (string.IsNullOrWhiteSpace(fullName))
        {
            return Result.Fail("Vui lòng nhập họ và tên bác sĩ.");
        }

        if (fullName.Length > 100)
        {
            return Result.Fail("Họ và tên không được vượt quá 100 ký tự.");
        }

        if (specialtyId <= 0)
        {
            return Result.Fail("Vui lòng chọn chuyên khoa.");
        }

        if (gender.HasValue && !Enum.IsDefined(gender.Value))
        {
            return Result.Fail("Giới tính không hợp lệ.");
        }

        if (dateOfBirth.HasValue && dateOfBirth.Value > DateOnly.FromDateTime(DateTime.Today))
        {
            return Result.Fail("Ngày sinh không được lớn hơn ngày hiện tại.");
        }

        if (phone is not null && phone.Length > 15)
        {
            return Result.Fail("Số điện thoại không được vượt quá 15 ký tự.");
        }

        if (degree is not null && degree.Length > 100)
        {
            return Result.Fail("Học vị / chức danh không được vượt quá 100 ký tự.");
        }

        if (experienceYears.HasValue && (experienceYears.Value < 0 || experienceYears.Value > 80))
        {
            return Result.Fail("Số năm kinh nghiệm phải từ 0 đến 80.");
        }

        if (bio is not null && bio.Length > 1000)
        {
            return Result.Fail("Giới thiệu không được vượt quá 1000 ký tự.");
        }

        if (avatarUrl is not null)
        {
            if (avatarUrl.Length > 300)
            {
                return Result.Fail("Đường dẫn ảnh đại diện không được vượt quá 300 ký tự.");
            }

            if (!BusinessValidationHelper.IsSafeUrl(avatarUrl))
            {
                return Result.Fail("Đường dẫn ảnh đại diện phải bắt đầu bằng '/', 'http://' hoặc 'https://'.");
            }
        }

        if (consultationFee < 0)
        {
            return Result.Fail("Phí khám không được nhỏ hơn 0.");
        }

        return Result.Ok();
    }

    #endregion

    private IQueryable<Doctor> BuildActiveDoctorQuery()
    {
        return _unitOfWork.Repository<Doctor>().Query()
            .AsNoTracking()
            .Where(d => d.IsActive && d.User.IsActive && d.Specialty.IsActive);
    }

    private static void RoundAverageRatings(List<BacSiPublicDto> doctors)
    {
        foreach (var doctor in doctors)
        {
            doctor.DiemDanhGiaTrungBinh = doctor.SoLuotDanhGia == 0
                ? 0.0
                : Math.Round(doctor.DiemDanhGiaTrungBinh, 1);
        }
    }
}
