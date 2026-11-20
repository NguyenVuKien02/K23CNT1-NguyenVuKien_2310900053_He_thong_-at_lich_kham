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
public class NhomThuocController : Controller
{
    private readonly IDanhMucService _danhMucService;

    public NhomThuocController(IDanhMucService danhMucService)
    {
        _danhMucService = danhMucService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? tuKhoa = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var paged = await _danhMucService.TimKiemNhomThuocAdminAsync(
            tuKhoa,
            page,
            PagedList<NhomThuocAdminDto>.DefaultPageSize,
            cancellationToken);

        var items = paged.Items
            .Select(x => new NhomThuocAdminItemViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                SoThuoc = x.SoThuoc
            })
            .ToList();

        var viewModel = new NhomThuocAdminIndexViewModel
        {
            TuKhoa = tuKhoa?.Trim(),
            DanhSach = new PagedList<NhomThuocAdminItemViewModel>(items, paged.TotalCount, paged.PageIndex, paged.PageSize)
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new NhomThuocFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NhomThuocFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _danhMucService.TaoNhomThuocAsync(
            new NhomThuocUpsertDto
            {
                Name = model.Name,
                Description = model.Description
            },
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Không thể tạo nhóm thuốc.");
            return View(model);
        }

        TempData["Success"] = "Thêm nhóm thuốc thành công!";
        return RedirectToAction(nameof(Index), new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await _danhMucService.LayNhomThuocTheoIdAsync(id, cancellationToken);
        if (!result.Success || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Không tìm thấy nhóm thuốc.";
            return RedirectToAction(nameof(Index), new { area = "Admin" });
        }

        var viewModel = new NhomThuocFormViewModel
        {
            Id = result.Data.Id,
            Name = result.Data.Name,
            Description = result.Data.Description
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(NhomThuocFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _danhMucService.CapNhatNhomThuocAsync(
            new NhomThuocUpsertDto
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description
            },
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Không thể cập nhật nhóm thuốc.");
            return View(model);
        }

        TempData["Success"] = "Cập nhật nhóm thuốc thành công!";
        return RedirectToAction(nameof(Index), new { area = "Admin" });
    }
}
