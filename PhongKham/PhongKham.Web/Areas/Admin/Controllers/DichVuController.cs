using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhongKham.Business.DTOs.DanhMuc;
using PhongKham.Business.Services;
using PhongKham.Common.Constants;
using PhongKham.Common.Enums;
using PhongKham.Common.Helpers;
using PhongKham.Web.ViewModels.Admin;

namespace PhongKham.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class DichVuController : Controller
{
    private readonly IDanhMucService _danhMucService;

    public DichVuController(IDanhMucService danhMucService)
    {
        _danhMucService = danhMucService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? tuKhoa = null,
        LoaiDichVu? loaiDichVu = null,
        bool? conHieuLuc = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var paged = await _danhMucService.TimKiemDichVuAdminAsync(
            new DichVuFilterDto
            {
                TuKhoa = tuKhoa,
                LoaiDichVu = loaiDichVu,
                ConHieuLuc = conHieuLuc,
                Page = page,
                PageSize = PagedList<DichVuAdminDto>.DefaultPageSize
            },
            cancellationToken);

        var items = paged.Items
            .Select(x => new DichVuAdminItemViewModel
            {
                Id = x.Id,
                MaDichVu = x.MaDichVu,
                TenDichVu = x.TenDichVu,
                LoaiDichVu = x.LoaiDichVu,
                DonGia = x.DonGia,
                MoTa = x.MoTa,
                ConHieuLuc = x.ConHieuLuc,
                SoLuotChiDinh = x.SoLuotChiDinh
            })
            .ToList();

        var viewModel = new DichVuAdminIndexViewModel
        {
            TuKhoa = tuKhoa?.Trim(),
            LoaiDichVu = loaiDichVu,
            ConHieuLuc = conHieuLuc,
            DanhSach = new PagedList<DichVuAdminItemViewModel>(items, paged.TotalCount, paged.PageIndex, paged.PageSize)
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new DichVuFormViewModel { ConHieuLuc = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DichVuFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _danhMucService.TaoDichVuAsync(
            new DichVuUpsertDto
            {
                MaDichVu = model.MaDichVu,
                TenDichVu = model.TenDichVu,
                LoaiDichVu = model.LoaiDichVu,
                DonGia = model.DonGia,
                MoTa = model.MoTa,
                ConHieuLuc = model.ConHieuLuc
            },
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Không thể thêm dịch vụ.");
            return View(model);
        }

        TempData["Success"] = "Thêm dịch vụ thành công!";
        return RedirectToAction(nameof(Index), new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await _danhMucService.LayDichVuTheoIdAsync(id, cancellationToken);
        if (!result.Success || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Không tìm thấy dịch vụ.";
            return RedirectToAction(nameof(Index), new { area = "Admin" });
        }

        var dto = result.Data;
        var viewModel = new DichVuFormViewModel
        {
            Id = dto.Id,
            MaDichVu = dto.MaDichVu,
            TenDichVu = dto.TenDichVu,
            LoaiDichVu = dto.LoaiDichVu,
            DonGia = dto.DonGia,
            MoTa = dto.MoTa,
            ConHieuLuc = dto.ConHieuLuc
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(DichVuFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _danhMucService.CapNhatDichVuAsync(
            new DichVuUpsertDto
            {
                Id = model.Id,
                MaDichVu = model.MaDichVu,
                TenDichVu = model.TenDichVu,
                LoaiDichVu = model.LoaiDichVu,
                DonGia = model.DonGia,
                MoTa = model.MoTa,
                ConHieuLuc = model.ConHieuLuc
            },
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Không thể cập nhật dịch vụ.");
            return View(model);
        }

        TempData["Success"] = "Cập nhật dịch vụ thành công!";
        return RedirectToAction(nameof(Index), new { area = "Admin" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiTrangThai(
        int id,
        bool conHieuLuc,
        string? tuKhoa = null,
        LoaiDichVu? loaiDichVu = null,
        bool? conHieuLucFilter = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await _danhMucService.DoiTrangThaiDichVuAsync(id, conHieuLuc, cancellationToken);
        if (!result.Success)
        {
            TempData["Error"] = result.Error ?? "Không thể cập nhật trạng thái dịch vụ.";
        }
        else
        {
            TempData["Success"] = conHieuLuc
                ? "Đã kích hoạt lại dịch vụ."
                : "Đã ngừng áp dụng dịch vụ.";
        }

        return RedirectToAction(nameof(Index), new { area = "Admin", tuKhoa, loaiDichVu, conHieuLuc = conHieuLucFilter, page });
    }
}
