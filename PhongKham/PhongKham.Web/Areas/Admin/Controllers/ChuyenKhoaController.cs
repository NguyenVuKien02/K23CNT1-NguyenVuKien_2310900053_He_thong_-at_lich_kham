using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhongKham.Business.DTOs.DanhMuc;
using PhongKham.Business.Services;
using PhongKham.Common.Constants;
using PhongKham.Common.Helpers;
using PhongKham.Web.ViewModels.Admin;

namespace PhongKham.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class ChuyenKhoaController : Controller
{
    private readonly IDanhMucService _danhMucService;

    public ChuyenKhoaController(IDanhMucService danhMucService)
    {
        _danhMucService = danhMucService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? tuKhoa = null,
        bool? trangThai = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var paged = await _danhMucService.TimKiemChuyenKhoaAdminAsync(
            tuKhoa,
            trangThai,
            page,
            PagedList<ChuyenKhoaAdminDto>.DefaultPageSize,
            cancellationToken);

        var items = paged.Items
            .Select(x => new ChuyenKhoaAdminItemViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive,
                SoBacSi = x.SoBacSi
            })
            .ToList();

        var viewModel = new ChuyenKhoaAdminIndexViewModel
        {
            TuKhoa = tuKhoa?.Trim(),
            TrangThai = trangThai,
            DanhSach = new PagedList<ChuyenKhoaAdminItemViewModel>(items, paged.TotalCount, paged.PageIndex, paged.PageSize)
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ChuyenKhoaFormViewModel { IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ChuyenKhoaFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _danhMucService.TaoChuyenKhoaAsync(
            new ChuyenKhoaUpsertDto
            {
                Name = model.Name,
                Description = model.Description,
                IsActive = model.IsActive
            },
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Không thể tạo chuyên khoa.");
            return View(model);
        }

        TempData["Success"] = "Thêm chuyên khoa thành công!";
        return RedirectToAction(nameof(Index), new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await _danhMucService.LayChuyenKhoaAdminTheoIdAsync(id, cancellationToken);
        if (!result.Success || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Không tìm thấy chuyên khoa.";
            return RedirectToAction(nameof(Index), new { area = "Admin" });
        }

        var viewModel = new ChuyenKhoaFormViewModel
        {
            Id = result.Data.Id,
            Name = result.Data.Name,
            Description = result.Data.Description,
            IsActive = result.Data.IsActive
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ChuyenKhoaFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _danhMucService.CapNhatChuyenKhoaAsync(
            new ChuyenKhoaUpsertDto
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                IsActive = model.IsActive
            },
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Không thể cập nhật chuyên khoa.");
            return View(model);
        }

        TempData["Success"] = "Cập nhật chuyên khoa thành công!";
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
        var result = await _danhMucService.DoiTrangThaiChuyenKhoaAsync(id, isActive, cancellationToken);
        if (!result.Success)
        {
            TempData["Error"] = result.Error ?? "Không thể cập nhật trạng thái chuyên khoa.";
        }
        else
        {
            TempData["Success"] = isActive
                ? "Đã kích hoạt lại chuyên khoa."
                : "Đã ngừng hoạt động chuyên khoa.";
        }

        return RedirectToAction(nameof(Index), new { area = "Admin", tuKhoa, trangThai, page });
    }
}
