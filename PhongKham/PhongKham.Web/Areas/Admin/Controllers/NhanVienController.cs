using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhongKham.Business.DTOs.Staffs;
using PhongKham.Business.Services;
using PhongKham.Common.Constants;
using PhongKham.Common.Helpers;
using PhongKham.Web.ViewModels.Admin;

namespace PhongKham.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class NhanVienController : Controller
{
    private readonly IStaffService _staffService;

    public NhanVienController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? tuKhoa = null,
        bool? trangThai = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var paged = await _staffService.TimKiemNhanVienAdminAsync(
            new NhanVienFilterDto
            {
                TuKhoa = tuKhoa,
                TrangThai = trangThai,
                Page = page,
                PageSize = PagedList<NhanVienAdminDto>.DefaultPageSize
            },
            cancellationToken);

        var items = paged.Items
            .Select(x => new NhanVienAdminItemViewModel
            {
                Id = x.Id,
                UserId = x.UserId,
                Email = x.Email,
                UserIsActive = x.UserIsActive,
                FullName = x.FullName,
                Gender = x.Gender,
                Phone = x.Phone,
                Position = x.Position,
                HireDate = x.HireDate,
                IsActive = x.IsActive
            })
            .ToList();

        var viewModel = new NhanVienAdminIndexViewModel
        {
            TuKhoa = tuKhoa?.Trim(),
            TrangThai = trangThai,
            DanhSach = new PagedList<NhanVienAdminItemViewModel>(items, paged.TotalCount, paged.PageIndex, paged.PageSize)
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new NhanVienCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NhanVienCreateViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _staffService.TaoNhanVienAsync(
            new NhanVienCreateDto
            {
                Email = model.Email,
                Password = model.Password,
                FullName = model.FullName,
                Gender = model.Gender,
                Phone = model.Phone,
                Position = model.Position,
                HireDate = model.HireDate,
                IsActive = model.IsActive
            },
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Không thể tạo nhân viên.");
            return View(model);
        }

        TempData["Success"] = "Tạo hồ sơ và tài khoản nhân viên lễ tân thành công!";
        return RedirectToAction(nameof(Index), new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await _staffService.LayNhanVienAdminTheoIdAsync(id, cancellationToken);
        if (!result.Success || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Không tìm thấy thông tin nhân viên.";
            return RedirectToAction(nameof(Index), new { area = "Admin" });
        }

        var dto = result.Data;
        var viewModel = new NhanVienEditViewModel
        {
            Id = dto.Id,
            UserId = dto.UserId,
            Email = dto.Email,
            FullName = dto.FullName,
            Gender = dto.Gender,
            Phone = dto.Phone,
            Position = dto.Position,
            HireDate = dto.HireDate,
            IsActive = dto.IsActive
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(NhanVienEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _staffService.CapNhatNhanVienAsync(
            new NhanVienUpdateDto
            {
                Id = model.Id,
                FullName = model.FullName,
                Gender = model.Gender,
                Phone = model.Phone,
                Position = model.Position,
                HireDate = model.HireDate,
                IsActive = model.IsActive
            },
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Không thể cập nhật thông tin nhân viên.");
            return View(model);
        }

        TempData["Success"] = "Cập nhật thông tin nhân viên thành công!";
        return RedirectToAction(nameof(Index), new { area = "Admin" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiTrangThai(
        int id,
        bool isActive,
        string? tuKhoa = null,
        bool? trangThai = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await _staffService.DoiTrangThaiNhanVienAsync(id, isActive, cancellationToken);
        if (!result.Success)
        {
            TempData["Error"] = result.Error ?? "Không thể cập nhật trạng thái nhân viên.";
        }
        else
        {
            TempData["Success"] = isActive
                ? "Đã kích hoạt lại hồ sơ và tài khoản nhân viên."
                : "Đã ngừng hoạt động nhân viên và khóa tài khoản đăng nhập.";
        }

        return RedirectToAction(nameof(Index), new { area = "Admin", tuKhoa, trangThai, page });
    }
}
