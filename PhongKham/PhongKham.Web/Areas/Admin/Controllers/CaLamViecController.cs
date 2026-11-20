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
public class CaLamViecController : Controller
{
    private readonly IDanhMucService _danhMucService;

    public CaLamViecController(IDanhMucService danhMucService)
    {
        _danhMucService = danhMucService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? tuKhoa = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var paged = await _danhMucService.TimKiemCaLamViecAdminAsync(
            tuKhoa,
            page,
            PagedList<CaLamViecAdminDto>.DefaultPageSize,
            cancellationToken);

        var items = paged.Items
            .Select(x => new CaLamViecAdminItemViewModel
            {
                Id = x.Id,
                TenCa = x.TenCa,
                GioBatDau = x.GioBatDau,
                GioKetThuc = x.GioKetThuc,
                ThoiLuongKhungGio = x.ThoiLuongKhungGio,
                SoKhungGio = x.SoKhungGio,
                SoLichLamViec = x.SoLichLamViec
            })
            .ToList();

        var viewModel = new CaLamViecAdminIndexViewModel
        {
            TuKhoa = tuKhoa?.Trim(),
            DanhSach = new PagedList<CaLamViecAdminItemViewModel>(items, paged.TotalCount, paged.PageIndex, paged.PageSize)
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CaLamViecFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CaLamViecFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _danhMucService.TaoCaLamViecAsync(
            new CaLamViecUpsertDto
            {
                TenCa = model.TenCa,
                GioBatDau = model.GioBatDau,
                GioKetThuc = model.GioKetThuc,
                ThoiLuongKhungGio = model.ThoiLuongKhungGio
            },
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Không thể thêm ca làm việc.");
            return View(model);
        }

        TempData["Success"] = "Thêm ca làm việc thành công!";
        return RedirectToAction(nameof(Index), new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await _danhMucService.LayCaLamViecTheoIdAsync(id, cancellationToken);
        if (!result.Success || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Không tìm thấy ca làm việc.";
            return RedirectToAction(nameof(Index), new { area = "Admin" });
        }

        var dto = result.Data;
        var viewModel = new CaLamViecFormViewModel
        {
            Id = dto.Id,
            TenCa = dto.TenCa,
            GioBatDau = dto.GioBatDau,
            GioKetThuc = dto.GioKetThuc,
            ThoiLuongKhungGio = dto.ThoiLuongKhungGio
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CaLamViecFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _danhMucService.CapNhatCaLamViecAsync(
            new CaLamViecUpsertDto
            {
                Id = model.Id,
                TenCa = model.TenCa,
                GioBatDau = model.GioBatDau,
                GioKetThuc = model.GioKetThuc,
                ThoiLuongKhungGio = model.ThoiLuongKhungGio
            },
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Không thể cập nhật ca làm việc.");
            return View(model);
        }

        TempData["Success"] = "Cập nhật ca làm việc thành công!";
        return RedirectToAction(nameof(Index), new { area = "Admin" });
    }
}
