using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhongKham.Business.DTOs.TaiKhoan;
using PhongKham.Business.Services;
using PhongKham.Common.Constants;
using PhongKham.Common.Helpers;
using PhongKham.Web.ViewModels.Admin;

namespace PhongKham.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class TaiKhoanController : Controller
{
    private readonly ITaiKhoanService _taiKhoanService;

    public TaiKhoanController(ITaiKhoanService taiKhoanService)
    {
        _taiKhoanService = taiKhoanService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? tuKhoa = null,
        string? roleName = null,
        bool? trangThai = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var paged = await _taiKhoanService.TimKiemTaiKhoanAdminAsync(
            new TaiKhoanFilterDto
            {
                TuKhoa = tuKhoa,
                RoleName = roleName,
                TrangThai = trangThai,
                Page = page,
                PageSize = PagedList<TaiKhoanAdminDto>.DefaultPageSize
            },
            cancellationToken);

        var items = paged.Items
            .Select(x => new TaiKhoanAdminItemViewModel
            {
                Id = x.Id,
                Email = x.Email,
                FullName = x.FullName,
                RoleName = x.RoleName,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                LastLoginAt = x.LastLoginAt
            })
            .ToList();

        var viewModel = new TaiKhoanAdminIndexViewModel
        {
            TuKhoa = tuKhoa?.Trim(),
            RoleName = roleName?.Trim(),
            TrangThai = trangThai,
            CurrentAdminUserId = GetCurrentUserId(),
            DanhSach = new PagedList<TaiKhoanAdminItemViewModel>(items, paged.TotalCount, paged.PageIndex, paged.PageSize)
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiTrangThai(
        int id,
        bool isActive,
        string? tuKhoa = null,
        string? roleName = null,
        bool? trangThai = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await _taiKhoanService.DoiTrangThaiTaiKhoanAsync(
            id,
            isActive,
            GetCurrentUserId(),
            cancellationToken);

        if (!result.Success)
        {
            TempData["Error"] = result.Error ?? "Không thể cập nhật trạng thái tài khoản.";
        }
        else
        {
            TempData["Success"] = isActive
                ? "Đã mở khóa tài khoản thành công."
                : "Đã khóa tài khoản thành công.";
        }

        return RedirectToAction(nameof(Index), new { area = "Admin", tuKhoa, roleName, trangThai, page });
    }

    [HttpGet]
    public async Task<IActionResult> DatLaiMatKhau(int id, CancellationToken cancellationToken)
    {
        var result = await _taiKhoanService.LayTaiKhoanTheoIdAsync(id, cancellationToken);
        if (!result.Success || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Không tìm thấy tài khoản.";
            return RedirectToAction(nameof(Index), new { area = "Admin" });
        }

        var dto = result.Data;
        var viewModel = new DatLaiMatKhauViewModel
        {
            UserId = dto.Id,
            Email = dto.Email,
            FullName = dto.FullName,
            RoleName = dto.RoleName
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DatLaiMatKhau(DatLaiMatKhauViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _taiKhoanService.DatLaiMatKhauAsync(
            new DatLaiMatKhauAdminDto
            {
                UserId = model.UserId,
                NewPassword = model.NewPassword
            },
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Không thể đặt lại mật khẩu.");
            return View(model);
        }

        TempData["Success"] = $"Đã đặt lại mật khẩu thành công cho tài khoản {model.Email}.";
        return RedirectToAction(nameof(Index), new { area = "Admin" });
    }

    private int GetCurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(raw, out var id) ? id : 0;
    }
}
