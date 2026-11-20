using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PhongKham.Business.DTOs.Doctors;
using PhongKham.Business.Services;
using PhongKham.Common.Constants;
using PhongKham.Common.Helpers;
using PhongKham.Web.ViewModels.Admin;

namespace PhongKham.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class BacSiController : Controller
{
    private readonly IDoctorService _doctorService;
    private readonly IDanhMucService _danhMucService;

    public BacSiController(IDoctorService doctorService, IDanhMucService danhMucService)
    {
        _doctorService = doctorService;
        _danhMucService = danhMucService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? tuKhoa = null,
        int? specialtyId = null,
        bool? trangThai = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var specialties = await _danhMucService.LayTatCaChuyenKhoaAsync(chiHoatDong: false, cancellationToken);
        var paged = await _doctorService.TimKiemBacSiAdminAsync(
            new BacSiAdminFilterDto
            {
                TuKhoa = tuKhoa,
                SpecialtyId = specialtyId,
                TrangThai = trangThai,
                Page = page,
                PageSize = PagedList<BacSiAdminDto>.DefaultPageSize
            },
            cancellationToken);

        var items = paged.Items
            .Select(x => new BacSiAdminItemViewModel
            {
                Id = x.Id,
                UserId = x.UserId,
                Email = x.Email,
                UserIsActive = x.UserIsActive,
                SpecialtyId = x.SpecialtyId,
                SpecialtyName = x.SpecialtyName,
                FullName = x.FullName,
                Gender = x.Gender,
                Phone = x.Phone,
                Degree = x.Degree,
                ExperienceYears = x.ExperienceYears,
                ConsultationFee = x.ConsultationFee,
                IsActive = x.IsActive
            })
            .ToList();

        var viewModel = new BacSiAdminIndexViewModel
        {
            TuKhoa = tuKhoa?.Trim(),
            SpecialtyId = specialtyId,
            TrangThai = trangThai,
            ChuyenKhoaOptions = specialties
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.IsActive ? s.Name : $"{s.Name} (Ngừng hoạt động)",
                    Selected = specialtyId == s.Id
                })
                .ToList(),
            DanhSach = new PagedList<BacSiAdminItemViewModel>(items, paged.TotalCount, paged.PageIndex, paged.PageSize)
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var viewModel = new BacSiCreateViewModel
        {
            ChuyenKhoaOptions = await BuildChuyenKhoaOptionsAsync(chiHoatDong: true, selectedId: null, cancellationToken)
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BacSiCreateViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.ChuyenKhoaOptions = await BuildChuyenKhoaOptionsAsync(chiHoatDong: true, model.SpecialtyId, cancellationToken);
            return View(model);
        }

        var result = await _doctorService.TaoBacSiAsync(
            new BacSiCreateDto
            {
                Email = model.Email,
                Password = model.Password,
                SpecialtyId = model.SpecialtyId,
                FullName = model.FullName,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Phone = model.Phone,
                Degree = model.Degree,
                ExperienceYears = model.ExperienceYears,
                Bio = model.Bio,
                AvatarUrl = model.AvatarUrl,
                ConsultationFee = model.ConsultationFee,
                IsActive = model.IsActive
            },
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Không thể tạo bác sĩ.");
            model.ChuyenKhoaOptions = await BuildChuyenKhoaOptionsAsync(chiHoatDong: true, model.SpecialtyId, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Tạo hồ sơ và tài khoản bác sĩ thành công!";
        return RedirectToAction(nameof(Index), new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await _doctorService.LayBacSiAdminTheoIdAsync(id, cancellationToken);
        if (!result.Success || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Không tìm thấy thông tin bác sĩ.";
            return RedirectToAction(nameof(Index), new { area = "Admin" });
        }

        var dto = result.Data;
        var viewModel = new BacSiEditViewModel
        {
            Id = dto.Id,
            UserId = dto.UserId,
            Email = dto.Email,
            SpecialtyId = dto.SpecialtyId,
            FullName = dto.FullName,
            Gender = dto.Gender,
            DateOfBirth = dto.DateOfBirth,
            Phone = dto.Phone,
            Degree = dto.Degree,
            ExperienceYears = dto.ExperienceYears,
            ConsultationFee = dto.ConsultationFee,
            AvatarUrl = dto.AvatarUrl,
            Bio = dto.Bio,
            IsActive = dto.IsActive,
            ChuyenKhoaOptions = await BuildChuyenKhoaOptionsAsync(chiHoatDong: false, dto.SpecialtyId, cancellationToken)
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BacSiEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.ChuyenKhoaOptions = await BuildChuyenKhoaOptionsAsync(chiHoatDong: false, model.SpecialtyId, cancellationToken);
            return View(model);
        }

        var result = await _doctorService.CapNhatBacSiAsync(
            new BacSiUpdateDto
            {
                Id = model.Id,
                SpecialtyId = model.SpecialtyId,
                FullName = model.FullName,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Phone = model.Phone,
                Degree = model.Degree,
                ExperienceYears = model.ExperienceYears,
                Bio = model.Bio,
                AvatarUrl = model.AvatarUrl,
                ConsultationFee = model.ConsultationFee,
                IsActive = model.IsActive
            },
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Không thể cập nhật thông tin bác sĩ.");
            model.ChuyenKhoaOptions = await BuildChuyenKhoaOptionsAsync(chiHoatDong: false, model.SpecialtyId, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Cập nhật thông tin bác sĩ thành công!";
        return RedirectToAction(nameof(Index), new { area = "Admin" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiTrangThai(
        int id,
        bool isActive,
        string? tuKhoa = null,
        int? specialtyId = null,
        bool? trangThai = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await _doctorService.DoiTrangThaiBacSiAsync(id, isActive, cancellationToken);
        if (!result.Success)
        {
            TempData["Error"] = result.Error ?? "Không thể cập nhật trạng thái bác sĩ.";
        }
        else
        {
            TempData["Success"] = isActive
                ? "Đã kích hoạt lại hồ sơ và tài khoản bác sĩ."
                : "Đã ngừng hoạt động bác sĩ và khóa tài khoản đăng nhập.";
        }

        return RedirectToAction(nameof(Index), new { area = "Admin", tuKhoa, specialtyId, trangThai, page });
    }

    private async Task<List<SelectListItem>> BuildChuyenKhoaOptionsAsync(
        bool chiHoatDong,
        int? selectedId,
        CancellationToken cancellationToken)
    {
        var specialties = await _danhMucService.LayTatCaChuyenKhoaAsync(chiHoatDong, cancellationToken);
        return specialties
            .Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.IsActive ? s.Name : $"{s.Name} (Ngừng hoạt động)",
                Selected = selectedId == s.Id
            })
            .ToList();
    }
}
