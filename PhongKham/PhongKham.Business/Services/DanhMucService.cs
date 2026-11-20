using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using PhongKham.Business.DTOs.DanhMuc;
using PhongKham.Business.Helpers;
using PhongKham.Common.Enums;
using PhongKham.Common.Helpers;
using PhongKham.Common.Results;
using PhongKham.Data.Entities;
using PhongKham.Data.Repositories;

namespace PhongKham.Business.Services;

public class DanhMucService : IDanhMucService
{
    private static readonly Regex MaDichVuRegex = new("^[A-Z0-9_-]+$", RegexOptions.Compiled);

    private readonly IUnitOfWork _unitOfWork;

    public DanhMucService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    #region Public

    public async Task<IReadOnlyList<ChuyenKhoaPublicDto>> LayChuyenKhoaHoatDongAsync(
        string? tuKhoa = null,
        CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Repository<Specialty>().Query()
            .AsNoTracking()
            .Where(s => s.IsActive);

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            query = query.Where(s =>
                s.Name.Contains(keyword) ||
                (s.Description != null && s.Description.Contains(keyword)));
        }

        return await query
            .OrderBy(s => s.Name)
            .Select(s => new ChuyenKhoaPublicDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                SoBacSi = s.Doctors.Count(d => d.IsActive && d.User.IsActive)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ChuyenKhoaPublicDto?> LayChuyenKhoaTheoIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return null;
        }

        return await _unitOfWork.Repository<Specialty>().Query()
            .AsNoTracking()
            .Where(s => s.IsActive && s.Id == id)
            .Select(s => new ChuyenKhoaPublicDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                SoBacSi = s.Doctors.Count(d => d.IsActive && d.User.IsActive)
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedList<DichVuPublicDto>> LayBangGiaDichVuAsync(
        LoaiDichVu? loaiDichVu = null,
        string? tuKhoa = null,
        int page = 1,
        int pageSize = PagedList<DichVuPublicDto>.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var normalizedPageSize = pageSize < 1 ? PagedList<DichVuPublicDto>.DefaultPageSize : pageSize;
        var normalizedPage = page < 1 ? 1 : page;

        var query = _unitOfWork.Repository<DichVu>().Query()
            .AsNoTracking()
            .Where(dv => dv.ConHieuLuc);

        if (loaiDichVu.HasValue && Enum.IsDefined(loaiDichVu.Value))
        {
            query = query.Where(dv => dv.LoaiDichVu == loaiDichVu.Value);
        }

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            query = query.Where(dv =>
                dv.MaDichVu.Contains(keyword) ||
                dv.TenDichVu.Contains(keyword) ||
                (dv.MoTa != null && dv.MoTa.Contains(keyword)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return PagedList<DichVuPublicDto>.Empty(normalizedPageSize);
        }

        normalizedPage = ClampPage(normalizedPage, totalCount, normalizedPageSize);

        var items = await query
            .OrderBy(dv => dv.LoaiDichVu)
            .ThenBy(dv => dv.TenDichVu)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(dv => new DichVuPublicDto
            {
                Id = dv.Id,
                MaDichVu = dv.MaDichVu,
                TenDichVu = dv.TenDichVu,
                LoaiDichVu = dv.LoaiDichVu,
                DonGia = dv.DonGia,
                MoTa = dv.MoTa
            })
            .ToListAsync(cancellationToken);

        return new PagedList<DichVuPublicDto>(items, totalCount, normalizedPage, normalizedPageSize);
    }

    public Task<int> DemDichVuHoatDongAsync(CancellationToken cancellationToken = default)
    {
        return _unitOfWork.Repository<DichVu>().Query()
            .AsNoTracking()
            .CountAsync(dv => dv.ConHieuLuc, cancellationToken);
    }

    #endregion

    #region Admin - Chuyên khoa

    public async Task<PagedList<ChuyenKhoaAdminDto>> TimKiemChuyenKhoaAdminAsync(
        string? tuKhoa = null,
        bool? trangThai = null,
        int page = 1,
        int pageSize = PagedList<ChuyenKhoaAdminDto>.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var normalizedPageSize = pageSize < 1 ? PagedList<ChuyenKhoaAdminDto>.DefaultPageSize : pageSize;
        var normalizedPage = page < 1 ? 1 : page;

        var query = _unitOfWork.Repository<Specialty>().Query().AsNoTracking();

        if (trangThai.HasValue)
        {
            query = query.Where(s => s.IsActive == trangThai.Value);
        }

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            query = query.Where(s =>
                s.Name.Contains(keyword) ||
                (s.Description != null && s.Description.Contains(keyword)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return PagedList<ChuyenKhoaAdminDto>.Empty(normalizedPageSize);
        }

        normalizedPage = ClampPage(normalizedPage, totalCount, normalizedPageSize);

        var items = await query
            .OrderByDescending(s => s.IsActive)
            .ThenBy(s => s.Name)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(s => new ChuyenKhoaAdminDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                IsActive = s.IsActive,
                SoBacSi = s.Doctors.Count
            })
            .ToListAsync(cancellationToken);

        return new PagedList<ChuyenKhoaAdminDto>(items, totalCount, normalizedPage, normalizedPageSize);
    }

    public async Task<IReadOnlyList<ChuyenKhoaAdminDto>> LayTatCaChuyenKhoaAsync(
        bool chiHoatDong = false,
        CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Repository<Specialty>().Query().AsNoTracking();
        if (chiHoatDong)
        {
            query = query.Where(s => s.IsActive);
        }

        return await query
            .OrderBy(s => s.Name)
            .Select(s => new ChuyenKhoaAdminDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                IsActive = s.IsActive,
                SoBacSi = s.Doctors.Count
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<ChuyenKhoaAdminDto>> LayChuyenKhoaAdminTheoIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<ChuyenKhoaAdminDto>.Fail("Không tìm thấy chuyên khoa.");
        }

        var item = await _unitOfWork.Repository<Specialty>().Query()
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new ChuyenKhoaAdminDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                IsActive = s.IsActive,
                SoBacSi = s.Doctors.Count
            })
            .FirstOrDefaultAsync(cancellationToken);

        return item is null
            ? Result<ChuyenKhoaAdminDto>.Fail("Không tìm thấy chuyên khoa.")
            : Result<ChuyenKhoaAdminDto>.Ok(item);
    }

    public async Task<Result<int>> TaoChuyenKhoaAsync(
        ChuyenKhoaUpsertDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = KiemTraChuyenKhoa(dto, out var name, out var description);
        if (!validation.Success)
        {
            return Result<int>.Fail(validation.Error!);
        }

        var repo = _unitOfWork.Repository<Specialty>();
        var exists = await repo.Query().AnyAsync(s => s.Name == name, cancellationToken);
        if (exists)
        {
            return Result<int>.Fail("Tên chuyên khoa đã tồn tại trong hệ thống.");
        }

        var entity = new Specialty
        {
            Name = name,
            Description = description,
            IsActive = dto.IsActive
        };

        try
        {
            await repo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Ok(entity.Id);
        }
        catch (DbUpdateException ex) when (BusinessValidationHelper.IsUniqueConstraintViolation(ex))
        {
            return Result<int>.Fail("Tên chuyên khoa đã tồn tại trong hệ thống.");
        }
    }

    public async Task<Result> CapNhatChuyenKhoaAsync(
        ChuyenKhoaUpsertDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Id <= 0)
        {
            return Result.Fail("Không tìm thấy chuyên khoa.");
        }

        var validation = KiemTraChuyenKhoa(dto, out var name, out var description);
        if (!validation.Success)
        {
            return validation;
        }

        var repo = _unitOfWork.Repository<Specialty>();
        var entity = await repo.GetByIdAsync(dto.Id, cancellationToken);
        if (entity is null)
        {
            return Result.Fail("Không tìm thấy chuyên khoa.");
        }

        var duplicate = await repo.Query()
            .AnyAsync(s => s.Id != dto.Id && s.Name == name, cancellationToken);
        if (duplicate)
        {
            return Result.Fail("Tên chuyên khoa đã tồn tại trong hệ thống.");
        }

        entity.Name = name;
        entity.Description = description;
        entity.IsActive = dto.IsActive;

        try
        {
            repo.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
        catch (DbUpdateException ex) when (BusinessValidationHelper.IsUniqueConstraintViolation(ex))
        {
            return Result.Fail("Tên chuyên khoa đã tồn tại trong hệ thống.");
        }
    }

    public async Task<Result> DoiTrangThaiChuyenKhoaAsync(
        int id,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result.Fail("Không tìm thấy chuyên khoa.");
        }

        var repo = _unitOfWork.Repository<Specialty>();
        var entity = await repo.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return Result.Fail("Không tìm thấy chuyên khoa.");
        }

        entity.IsActive = isActive;
        repo.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }

    private static Result KiemTraChuyenKhoa(
        ChuyenKhoaUpsertDto dto,
        out string name,
        out string? description)
    {
        name = (dto.Name ?? string.Empty).Trim();
        description = BusinessValidationHelper.TrimOrNull(dto.Description);

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Fail("Vui lòng nhập tên chuyên khoa.");
        }

        if (name.Length > 100)
        {
            return Result.Fail("Tên chuyên khoa không được vượt quá 100 ký tự.");
        }

        if (description is not null && description.Length > 500)
        {
            return Result.Fail("Mô tả không được vượt quá 500 ký tự.");
        }

        return Result.Ok();
    }

    #endregion

    #region Admin - Nhóm thuốc

    public async Task<PagedList<NhomThuocAdminDto>> TimKiemNhomThuocAdminAsync(
        string? tuKhoa = null,
        int page = 1,
        int pageSize = PagedList<NhomThuocAdminDto>.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var normalizedPageSize = pageSize < 1 ? PagedList<NhomThuocAdminDto>.DefaultPageSize : pageSize;
        var normalizedPage = page < 1 ? 1 : page;

        var query = _unitOfWork.Repository<MedicineCategory>().Query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            query = query.Where(c =>
                c.Name.Contains(keyword) ||
                (c.Description != null && c.Description.Contains(keyword)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return PagedList<NhomThuocAdminDto>.Empty(normalizedPageSize);
        }

        normalizedPage = ClampPage(normalizedPage, totalCount, normalizedPageSize);

        var items = await query
            .OrderBy(c => c.Name)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(c => new NhomThuocAdminDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                SoThuoc = c.Medicines.Count
            })
            .ToListAsync(cancellationToken);

        return new PagedList<NhomThuocAdminDto>(items, totalCount, normalizedPage, normalizedPageSize);
    }

    public async Task<IReadOnlyList<NhomThuocOptionDto>> LayTatCaNhomThuocAsync(
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Repository<MedicineCategory>().Query()
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new NhomThuocOptionDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<NhomThuocAdminDto>> LayNhomThuocTheoIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<NhomThuocAdminDto>.Fail("Không tìm thấy nhóm thuốc.");
        }

        var item = await _unitOfWork.Repository<MedicineCategory>().Query()
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new NhomThuocAdminDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                SoThuoc = c.Medicines.Count
            })
            .FirstOrDefaultAsync(cancellationToken);

        return item is null
            ? Result<NhomThuocAdminDto>.Fail("Không tìm thấy nhóm thuốc.")
            : Result<NhomThuocAdminDto>.Ok(item);
    }

    public async Task<Result<int>> TaoNhomThuocAsync(
        NhomThuocUpsertDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = KiemTraNhomThuoc(dto, out var name, out var description);
        if (!validation.Success)
        {
            return Result<int>.Fail(validation.Error!);
        }

        var repo = _unitOfWork.Repository<MedicineCategory>();
        var exists = await repo.Query().AnyAsync(c => c.Name == name, cancellationToken);
        if (exists)
        {
            return Result<int>.Fail("Tên nhóm thuốc đã tồn tại trong hệ thống.");
        }

        var entity = new MedicineCategory
        {
            Name = name,
            Description = description
        };

        try
        {
            await repo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Ok(entity.Id);
        }
        catch (DbUpdateException ex) when (BusinessValidationHelper.IsUniqueConstraintViolation(ex))
        {
            return Result<int>.Fail("Tên nhóm thuốc đã tồn tại trong hệ thống.");
        }
    }

    public async Task<Result> CapNhatNhomThuocAsync(
        NhomThuocUpsertDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Id <= 0)
        {
            return Result.Fail("Không tìm thấy nhóm thuốc.");
        }

        var validation = KiemTraNhomThuoc(dto, out var name, out var description);
        if (!validation.Success)
        {
            return validation;
        }

        var repo = _unitOfWork.Repository<MedicineCategory>();
        var entity = await repo.GetByIdAsync(dto.Id, cancellationToken);
        if (entity is null)
        {
            return Result.Fail("Không tìm thấy nhóm thuốc.");
        }

        var duplicate = await repo.Query()
            .AnyAsync(c => c.Id != dto.Id && c.Name == name, cancellationToken);
        if (duplicate)
        {
            return Result.Fail("Tên nhóm thuốc đã tồn tại trong hệ thống.");
        }

        entity.Name = name;
        entity.Description = description;

        try
        {
            repo.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
        catch (DbUpdateException ex) when (BusinessValidationHelper.IsUniqueConstraintViolation(ex))
        {
            return Result.Fail("Tên nhóm thuốc đã tồn tại trong hệ thống.");
        }
    }

    private static Result KiemTraNhomThuoc(
        NhomThuocUpsertDto dto,
        out string name,
        out string? description)
    {
        name = (dto.Name ?? string.Empty).Trim();
        description = BusinessValidationHelper.TrimOrNull(dto.Description);

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Fail("Vui lòng nhập tên nhóm thuốc.");
        }

        if (name.Length > 100)
        {
            return Result.Fail("Tên nhóm thuốc không được vượt quá 100 ký tự.");
        }

        if (description is not null && description.Length > 300)
        {
            return Result.Fail("Mô tả không được vượt quá 300 ký tự.");
        }

        return Result.Ok();
    }

    #endregion

    #region Admin - Thuốc

    public async Task<PagedList<ThuocAdminDto>> TimKiemThuocAdminAsync(
        ThuocFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        filter ??= new ThuocFilterDto();
        var normalizedPageSize = filter.PageSize < 1 ? PagedList<ThuocAdminDto>.DefaultPageSize : filter.PageSize;
        var normalizedPage = filter.Page < 1 ? 1 : filter.Page;

        var query = _unitOfWork.Repository<Medicine>().Query().AsNoTracking();

        if (filter.MedicineCategoryId.HasValue && filter.MedicineCategoryId.Value > 0)
        {
            var catId = filter.MedicineCategoryId.Value;
            query = query.Where(m => m.MedicineCategoryId == catId);
        }

        if (filter.TrangThai.HasValue)
        {
            query = query.Where(m => m.IsActive == filter.TrangThai.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.TrangThaiTonKho))
        {
            switch (filter.TrangThaiTonKho.Trim().ToLowerInvariant())
            {
                case "het":
                    query = query.Where(m => m.StockQuantity <= 0);
                    break;
                case "sap-het":
                    query = query.Where(m => m.StockQuantity > 0 && m.StockQuantity <= 10);
                    break;
                case "con":
                    query = query.Where(m => m.StockQuantity > 10);
                    break;
            }
        }

        if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
        {
            var keyword = filter.TuKhoa.Trim();
            query = query.Where(m =>
                m.Name.Contains(keyword) ||
                (m.ActiveIngredient != null && m.ActiveIngredient.Contains(keyword)) ||
                (m.Manufacturer != null && m.Manufacturer.Contains(keyword)) ||
                m.MedicineCategory.Name.Contains(keyword));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return PagedList<ThuocAdminDto>.Empty(normalizedPageSize);
        }

        normalizedPage = ClampPage(normalizedPage, totalCount, normalizedPageSize);

        var items = await query
            .OrderByDescending(m => m.IsActive)
            .ThenBy(m => m.Name)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(m => new ThuocAdminDto
            {
                Id = m.Id,
                MedicineCategoryId = m.MedicineCategoryId,
                MedicineCategoryName = m.MedicineCategory.Name,
                Name = m.Name,
                ActiveIngredient = m.ActiveIngredient,
                Unit = m.Unit,
                Price = m.Price,
                StockQuantity = m.StockQuantity,
                Manufacturer = m.Manufacturer,
                IsActive = m.IsActive
            })
            .ToListAsync(cancellationToken);

        return new PagedList<ThuocAdminDto>(items, totalCount, normalizedPage, normalizedPageSize);
    }

    public async Task<Result<ThuocAdminDto>> LayThuocTheoIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<ThuocAdminDto>.Fail("Không tìm thấy thuốc.");
        }

        var item = await _unitOfWork.Repository<Medicine>().Query()
            .AsNoTracking()
            .Where(m => m.Id == id)
            .Select(m => new ThuocAdminDto
            {
                Id = m.Id,
                MedicineCategoryId = m.MedicineCategoryId,
                MedicineCategoryName = m.MedicineCategory.Name,
                Name = m.Name,
                ActiveIngredient = m.ActiveIngredient,
                Unit = m.Unit,
                Price = m.Price,
                StockQuantity = m.StockQuantity,
                Manufacturer = m.Manufacturer,
                IsActive = m.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);

        return item is null
            ? Result<ThuocAdminDto>.Fail("Không tìm thấy thuốc.")
            : Result<ThuocAdminDto>.Ok(item);
    }

    public async Task<Result<int>> TaoThuocAsync(
        ThuocUpsertDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = KiemTraThuoc(dto, out var name, out var activeIngredient, out var unit, out var manufacturer);
        if (!validation.Success)
        {
            return Result<int>.Fail(validation.Error!);
        }

        var categoryExists = await _unitOfWork.Repository<MedicineCategory>().Query()
            .AnyAsync(c => c.Id == dto.MedicineCategoryId, cancellationToken);
        if (!categoryExists)
        {
            return Result<int>.Fail("Nhóm thuốc đã chọn không tồn tại.");
        }

        var entity = new Medicine
        {
            MedicineCategoryId = dto.MedicineCategoryId,
            Name = name,
            ActiveIngredient = activeIngredient,
            Unit = unit,
            Price = Math.Truncate(dto.Price),
            StockQuantity = dto.StockQuantity,
            Manufacturer = manufacturer,
            IsActive = dto.IsActive
        };

        await _unitOfWork.Repository<Medicine>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(entity.Id);
    }

    public async Task<Result> CapNhatThuocAsync(
        ThuocUpsertDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Id <= 0)
        {
            return Result.Fail("Không tìm thấy thuốc.");
        }

        var validation = KiemTraThuoc(dto, out var name, out var activeIngredient, out var unit, out var manufacturer);
        if (!validation.Success)
        {
            return validation;
        }

        var repo = _unitOfWork.Repository<Medicine>();
        var entity = await repo.GetByIdAsync(dto.Id, cancellationToken);
        if (entity is null)
        {
            return Result.Fail("Không tìm thấy thuốc.");
        }

        var categoryExists = await _unitOfWork.Repository<MedicineCategory>().Query()
            .AnyAsync(c => c.Id == dto.MedicineCategoryId, cancellationToken);
        if (!categoryExists)
        {
            return Result.Fail("Nhóm thuốc đã chọn không tồn tại.");
        }

        entity.MedicineCategoryId = dto.MedicineCategoryId;
        entity.Name = name;
        entity.ActiveIngredient = activeIngredient;
        entity.Unit = unit;
        entity.Price = Math.Truncate(dto.Price);
        entity.StockQuantity = dto.StockQuantity;
        entity.Manufacturer = manufacturer;
        entity.IsActive = dto.IsActive;

        repo.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }

    public async Task<Result> CapNhatTonKhoThuocAsync(
        int id,
        int stockQuantity,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result.Fail("Không tìm thấy thuốc.");
        }

        if (stockQuantity < 0)
        {
            return Result.Fail("Số lượng tồn kho không được nhỏ hơn 0.");
        }

        var repo = _unitOfWork.Repository<Medicine>();
        var entity = await repo.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return Result.Fail("Không tìm thấy thuốc.");
        }

        entity.StockQuantity = stockQuantity;
        repo.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }

    public async Task<Result> DoiTrangThaiThuocAsync(
        int id,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result.Fail("Không tìm thấy thuốc.");
        }

        var repo = _unitOfWork.Repository<Medicine>();
        var entity = await repo.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return Result.Fail("Không tìm thấy thuốc.");
        }

        entity.IsActive = isActive;
        repo.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }

    private static Result KiemTraThuoc(
        ThuocUpsertDto dto,
        out string name,
        out string? activeIngredient,
        out string unit,
        out string? manufacturer)
    {
        name = (dto.Name ?? string.Empty).Trim();
        activeIngredient = BusinessValidationHelper.TrimOrNull(dto.ActiveIngredient);
        unit = (dto.Unit ?? string.Empty).Trim();
        manufacturer = BusinessValidationHelper.TrimOrNull(dto.Manufacturer);

        if (dto.MedicineCategoryId <= 0)
        {
            return Result.Fail("Vui lòng chọn nhóm thuốc.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Fail("Vui lòng nhập tên thuốc.");
        }

        if (name.Length > 150)
        {
            return Result.Fail("Tên thuốc không được vượt quá 150 ký tự.");
        }

        if (activeIngredient is not null && activeIngredient.Length > 200)
        {
            return Result.Fail("Hoạt chất không được vượt quá 200 ký tự.");
        }

        if (string.IsNullOrWhiteSpace(unit))
        {
            return Result.Fail("Vui lòng nhập đơn vị tính.");
        }

        if (unit.Length > 30)
        {
            return Result.Fail("Đơn vị tính không được vượt quá 30 ký tự.");
        }

        if (dto.Price < 0)
        {
            return Result.Fail("Đơn giá không được nhỏ hơn 0.");
        }

        if (dto.StockQuantity < 0)
        {
            return Result.Fail("Số lượng tồn kho không được nhỏ hơn 0.");
        }

        if (manufacturer is not null && manufacturer.Length > 150)
        {
            return Result.Fail("Hãng sản xuất không được vượt quá 150 ký tự.");
        }

        return Result.Ok();
    }

    #endregion

    #region Admin - Dịch vụ

    public async Task<PagedList<DichVuAdminDto>> TimKiemDichVuAdminAsync(
        DichVuFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        filter ??= new DichVuFilterDto();
        var normalizedPageSize = filter.PageSize < 1 ? PagedList<DichVuAdminDto>.DefaultPageSize : filter.PageSize;
        var normalizedPage = filter.Page < 1 ? 1 : filter.Page;

        var query = _unitOfWork.Repository<DichVu>().Query().AsNoTracking();

        if (filter.LoaiDichVu.HasValue && Enum.IsDefined(filter.LoaiDichVu.Value))
        {
            query = query.Where(dv => dv.LoaiDichVu == filter.LoaiDichVu.Value);
        }

        if (filter.ConHieuLuc.HasValue)
        {
            query = query.Where(dv => dv.ConHieuLuc == filter.ConHieuLuc.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
        {
            var keyword = filter.TuKhoa.Trim();
            query = query.Where(dv =>
                dv.MaDichVu.Contains(keyword) ||
                dv.TenDichVu.Contains(keyword) ||
                (dv.MoTa != null && dv.MoTa.Contains(keyword)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return PagedList<DichVuAdminDto>.Empty(normalizedPageSize);
        }

        normalizedPage = ClampPage(normalizedPage, totalCount, normalizedPageSize);

        var items = await query
            .OrderByDescending(dv => dv.ConHieuLuc)
            .ThenBy(dv => dv.LoaiDichVu)
            .ThenBy(dv => dv.TenDichVu)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(dv => new DichVuAdminDto
            {
                Id = dv.Id,
                MaDichVu = dv.MaDichVu,
                TenDichVu = dv.TenDichVu,
                LoaiDichVu = dv.LoaiDichVu,
                DonGia = dv.DonGia,
                MoTa = dv.MoTa,
                ConHieuLuc = dv.ConHieuLuc,
                SoLuotChiDinh = dv.ChiDinhDichVus.Count
            })
            .ToListAsync(cancellationToken);

        return new PagedList<DichVuAdminDto>(items, totalCount, normalizedPage, normalizedPageSize);
    }

    public async Task<Result<DichVuAdminDto>> LayDichVuTheoIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<DichVuAdminDto>.Fail("Không tìm thấy dịch vụ.");
        }

        var item = await _unitOfWork.Repository<DichVu>().Query()
            .AsNoTracking()
            .Where(dv => dv.Id == id)
            .Select(dv => new DichVuAdminDto
            {
                Id = dv.Id,
                MaDichVu = dv.MaDichVu,
                TenDichVu = dv.TenDichVu,
                LoaiDichVu = dv.LoaiDichVu,
                DonGia = dv.DonGia,
                MoTa = dv.MoTa,
                ConHieuLuc = dv.ConHieuLuc,
                SoLuotChiDinh = dv.ChiDinhDichVus.Count
            })
            .FirstOrDefaultAsync(cancellationToken);

        return item is null
            ? Result<DichVuAdminDto>.Fail("Không tìm thấy dịch vụ.")
            : Result<DichVuAdminDto>.Ok(item);
    }

    public async Task<Result<int>> TaoDichVuAsync(
        DichVuUpsertDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = KiemTraDichVu(dto, isCreate: true, out var maDichVu, out var tenDichVu, out var moTa);
        if (!validation.Success)
        {
            return Result<int>.Fail(validation.Error!);
        }

        var repo = _unitOfWork.Repository<DichVu>();
        var autoGenerateCode = string.IsNullOrEmpty(maDichVu);

        if (!autoGenerateCode)
        {
            var exists = await repo.Query().AnyAsync(dv => dv.MaDichVu == maDichVu, cancellationToken);
            if (exists)
            {
                return Result<int>.Fail("Mã dịch vụ đã tồn tại trong hệ thống.");
            }
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var entity = new DichVu
            {
                MaDichVu = autoGenerateCode ? BusinessValidationHelper.TaoMaTam("DV") : maDichVu!,
                TenDichVu = tenDichVu,
                LoaiDichVu = dto.LoaiDichVu,
                DonGia = Math.Truncate(dto.DonGia),
                MoTa = moTa,
                ConHieuLuc = dto.ConHieuLuc
            };

            await repo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (autoGenerateCode)
            {
                entity.MaDichVu = $"DV{entity.Id:D6}";
                repo.Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            await _unitOfWork.CommitAsync(cancellationToken);
            return Result<int>.Ok(entity.Id);
        }
        catch (DbUpdateException ex) when (BusinessValidationHelper.IsUniqueConstraintViolation(ex))
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<int>.Fail("Mã dịch vụ đã tồn tại trong hệ thống.");
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Result> CapNhatDichVuAsync(
        DichVuUpsertDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Id <= 0)
        {
            return Result.Fail("Không tìm thấy dịch vụ.");
        }

        var validation = KiemTraDichVu(dto, isCreate: false, out _, out var tenDichVu, out var moTa);
        if (!validation.Success)
        {
            return validation;
        }

        var repo = _unitOfWork.Repository<DichVu>();
        var entity = await repo.GetByIdAsync(dto.Id, cancellationToken);
        if (entity is null)
        {
            return Result.Fail("Không tìm thấy dịch vụ.");
        }

        entity.TenDichVu = tenDichVu;
        entity.LoaiDichVu = dto.LoaiDichVu;
        entity.DonGia = Math.Truncate(dto.DonGia);
        entity.MoTa = moTa;
        entity.ConHieuLuc = dto.ConHieuLuc;

        repo.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }

    public async Task<Result> DoiTrangThaiDichVuAsync(
        int id,
        bool conHieuLuc,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result.Fail("Không tìm thấy dịch vụ.");
        }

        var repo = _unitOfWork.Repository<DichVu>();
        var entity = await repo.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return Result.Fail("Không tìm thấy dịch vụ.");
        }

        entity.ConHieuLuc = conHieuLuc;
        repo.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }

    private static Result KiemTraDichVu(
        DichVuUpsertDto dto,
        bool isCreate,
        out string? maDichVu,
        out string tenDichVu,
        out string? moTa)
    {
        maDichVu = string.IsNullOrWhiteSpace(dto.MaDichVu)
            ? null
            : dto.MaDichVu.Trim().ToUpperInvariant();
        tenDichVu = (dto.TenDichVu ?? string.Empty).Trim();
        moTa = BusinessValidationHelper.TrimOrNull(dto.MoTa);

        if (isCreate && maDichVu is not null)
        {
            if (maDichVu.Length > 20)
            {
                return Result.Fail("Mã dịch vụ không được vượt quá 20 ký tự.");
            }

            if (!MaDichVuRegex.IsMatch(maDichVu))
            {
                return Result.Fail("Mã dịch vụ chỉ được chứa chữ không dấu, chữ số, dấu gạch ngang (-) hoặc gạch dưới (_).");
            }
        }

        if (string.IsNullOrWhiteSpace(tenDichVu))
        {
            return Result.Fail("Vui lòng nhập tên dịch vụ.");
        }

        if (tenDichVu.Length > 200)
        {
            return Result.Fail("Tên dịch vụ không được vượt quá 200 ký tự.");
        }

        if (!Enum.IsDefined(dto.LoaiDichVu))
        {
            return Result.Fail("Loại dịch vụ không hợp lệ.");
        }

        if (dto.DonGia < 0)
        {
            return Result.Fail("Đơn giá không được nhỏ hơn 0.");
        }

        if (moTa is not null && moTa.Length > 500)
        {
            return Result.Fail("Mô tả không được vượt quá 500 ký tự.");
        }

        return Result.Ok();
    }

    #endregion

    #region Admin - Ca làm việc

    public async Task<PagedList<CaLamViecAdminDto>> TimKiemCaLamViecAdminAsync(
        string? tuKhoa = null,
        int page = 1,
        int pageSize = PagedList<CaLamViecAdminDto>.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var normalizedPageSize = pageSize < 1 ? PagedList<CaLamViecAdminDto>.DefaultPageSize : pageSize;
        var normalizedPage = page < 1 ? 1 : page;

        var query = _unitOfWork.Repository<CaLamViec>().Query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            query = query.Where(c => c.TenCa.Contains(keyword));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return PagedList<CaLamViecAdminDto>.Empty(normalizedPageSize);
        }

        normalizedPage = ClampPage(normalizedPage, totalCount, normalizedPageSize);

        var items = await query
            .OrderBy(c => c.GioBatDau)
            .ThenBy(c => c.TenCa)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(c => new CaLamViecAdminDto
            {
                Id = c.Id,
                TenCa = c.TenCa,
                GioBatDau = c.GioBatDau,
                GioKetThuc = c.GioKetThuc,
                ThoiLuongKhungGio = c.ThoiLuongKhungGio,
                SoLichLamViec = c.LichLamViecs.Count
            })
            .ToListAsync(cancellationToken);

        return new PagedList<CaLamViecAdminDto>(items, totalCount, normalizedPage, normalizedPageSize);
    }

    public async Task<Result<CaLamViecAdminDto>> LayCaLamViecTheoIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<CaLamViecAdminDto>.Fail("Không tìm thấy ca làm việc.");
        }

        var item = await _unitOfWork.Repository<CaLamViec>().Query()
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CaLamViecAdminDto
            {
                Id = c.Id,
                TenCa = c.TenCa,
                GioBatDau = c.GioBatDau,
                GioKetThuc = c.GioKetThuc,
                ThoiLuongKhungGio = c.ThoiLuongKhungGio,
                SoLichLamViec = c.LichLamViecs.Count
            })
            .FirstOrDefaultAsync(cancellationToken);

        return item is null
            ? Result<CaLamViecAdminDto>.Fail("Không tìm thấy ca làm việc.")
            : Result<CaLamViecAdminDto>.Ok(item);
    }

    public async Task<Result<int>> TaoCaLamViecAsync(
        CaLamViecUpsertDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = KiemTraCaLamViec(dto, out var tenCa);
        if (!validation.Success)
        {
            return Result<int>.Fail(validation.Error!);
        }

        var repo = _unitOfWork.Repository<CaLamViec>();
        var exists = await repo.Query().AnyAsync(c => c.TenCa == tenCa, cancellationToken);
        if (exists)
        {
            return Result<int>.Fail("Tên ca làm việc đã tồn tại trong hệ thống.");
        }

        var entity = new CaLamViec
        {
            TenCa = tenCa,
            GioBatDau = dto.GioBatDau,
            GioKetThuc = dto.GioKetThuc,
            ThoiLuongKhungGio = dto.ThoiLuongKhungGio
        };

        await repo.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(entity.Id);
    }

    public async Task<Result> CapNhatCaLamViecAsync(
        CaLamViecUpsertDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Id <= 0)
        {
            return Result.Fail("Không tìm thấy ca làm việc.");
        }

        var validation = KiemTraCaLamViec(dto, out var tenCa);
        if (!validation.Success)
        {
            return validation;
        }

        var repo = _unitOfWork.Repository<CaLamViec>();
        var entity = await repo.GetByIdAsync(dto.Id, cancellationToken);
        if (entity is null)
        {
            return Result.Fail("Không tìm thấy ca làm việc.");
        }

        var duplicate = await repo.Query()
            .AnyAsync(c => c.Id != dto.Id && c.TenCa == tenCa, cancellationToken);
        if (duplicate)
        {
            return Result.Fail("Tên ca làm việc đã tồn tại trong hệ thống.");
        }

        entity.TenCa = tenCa;
        entity.GioBatDau = dto.GioBatDau;
        entity.GioKetThuc = dto.GioKetThuc;
        entity.ThoiLuongKhungGio = dto.ThoiLuongKhungGio;

        repo.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }

    private static Result KiemTraCaLamViec(CaLamViecUpsertDto dto, out string tenCa)
    {
        tenCa = (dto.TenCa ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(tenCa))
        {
            return Result.Fail("Vui lòng nhập tên ca làm việc.");
        }

        if (tenCa.Length > 50)
        {
            return Result.Fail("Tên ca làm việc không được vượt quá 50 ký tự.");
        }

        if (dto.GioKetThuc <= dto.GioBatDau)
        {
            return Result.Fail("Giờ kết thúc phải lớn hơn giờ bắt đầu.");
        }

        if (dto.ThoiLuongKhungGio < 5 || dto.ThoiLuongKhungGio > 240)
        {
            return Result.Fail("Thời lượng khung giờ phải từ 5 đến 240 phút.");
        }

        var totalMinutes = (dto.GioKetThuc - dto.GioBatDau).TotalMinutes;
        if (dto.ThoiLuongKhungGio > totalMinutes)
        {
            return Result.Fail("Thời lượng khung giờ không được lớn hơn tổng thời gian của ca làm việc.");
        }

        return Result.Ok();
    }

    #endregion

    private static int ClampPage(int page, int totalCount, int pageSize)
    {
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        return page > totalPages ? totalPages : page;
    }
}
