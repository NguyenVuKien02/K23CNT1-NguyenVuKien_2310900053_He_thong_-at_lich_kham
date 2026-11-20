using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PhongKham.Business.DTOs.DanhMuc;
using PhongKham.Business.Services;
using PhongKham.Common.Constants;
using PhongKham.Common.Helpers;
using PhongKham.Web.ViewModels.Admin;

namespace PhongKham.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class ThuocController : Controller
{
    private readonly IDanhMucService _danhMucService;

    public ThuocController(IDanhMucService danhMucService)
    {
        _danhMucService = danhMucService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? tuKhoa = null,
        int? medicineCategoryId = null,
        bool? trangThai = null,
        string? trangThaiTonKho = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var categories = await _danhMucService.LayTatCaNhomThuocAsync(cancellationToken);
        var paged = await _danhMucService.TimKiemThuocAdminAsync(
            new ThuocFilterDto
            {
                TuKhoa = tuKhoa,
                MedicineCategoryId = medicineCategoryId,
                TrangThai = trangThai,
                TrangThaiTonKho = trangThaiTonKho,
                Page = page,
                PageSize = PagedList<ThuocAdminDto>.DefaultPageSize
            },
            cancellationToken);

        var items = paged.Items
            .Select(x => new ThuocAdminItemViewModel
            {
                Id = x.Id,
                MedicineCategoryId = x.MedicineCategoryId,
                MedicineCategoryName = x.MedicineCategoryName,
                Name = x.Name,
                ActiveIngredient = x.ActiveIngredient,
                Unit = x.Unit,
                Price = x.Price,
                StockQuantity = x.StockQuantity,
                Manufacturer = x.Manufacturer,
                IsActive = x.IsActive
            })
            .ToList();

        var viewModel = new ThuocAdminIndexViewModel
        {
            TuKhoa = tuKhoa?.Trim(),
            MedicineCategoryId = medicineCategoryId,
            TrangThai = trangThai,
            TrangThaiTonKho = trangThaiTonKho?.Trim(),
            NhomThuocOptions = categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = medicineCategoryId == c.Id
                })
                .ToList(),
            DanhSach = new PagedList<ThuocAdminItemViewModel>(items, paged.TotalCount, paged.PageIndex, paged.PageSize)
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var viewModel = new ThuocFormViewModel
        {
            IsActive = true,
            NhomThuocOptions = await BuildNhomThuocOptionsAsync(null, cancellationToken)
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ThuocFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.NhomThuocOptions = await BuildNhomThuocOptionsAsync(model.MedicineCategoryId, cancellationToken);
            return View(model);
        }

        var result = await _danhMucService.TaoThuocAsync(
            new ThuocUpsertDto
            {
                MedicineCategoryId = model.MedicineCategoryId,
                Name = model.Name,
                ActiveIngredient = model.ActiveIngredient,
                Unit = model.Unit,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                Manufacturer = model.Manufacturer,
                IsActive = model.IsActive
            },
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Không thể thêm thuốc.");
            model.NhomThuocOptions = await BuildNhomThuocOptionsAsync(model.MedicineCategoryId, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Thêm thuốc thành công!";
        return RedirectToAction(nameof(Index), new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await _danhMucService.LayThuocTheoIdAsync(id, cancellationToken);
        if (!result.Success || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Không tìm thấy thuốc.";
            return RedirectToAction(nameof(Index), new { area = "Admin" });
        }

        var dto = result.Data;
        var viewModel = new ThuocFormViewModel
        {
            Id = dto.Id,
            MedicineCategoryId = dto.MedicineCategoryId,
            Name = dto.Name,
            ActiveIngredient = dto.ActiveIngredient,
            Unit = dto.Unit,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            Manufacturer = dto.Manufacturer,
            IsActive = dto.IsActive,
            NhomThuocOptions = await BuildNhomThuocOptionsAsync(dto.MedicineCategoryId, cancellationToken)
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ThuocFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.NhomThuocOptions = await BuildNhomThuocOptionsAsync(model.MedicineCategoryId, cancellationToken);
            return View(model);
        }

        var result = await _danhMucService.CapNhatThuocAsync(
            new ThuocUpsertDto
            {
                Id = model.Id,
                MedicineCategoryId = model.MedicineCategoryId,
                Name = model.Name,
                ActiveIngredient = model.ActiveIngredient,
                Unit = model.Unit,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                Manufacturer = model.Manufacturer,
                IsActive = model.IsActive
            },
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Không thể cập nhật thuốc.");
            model.NhomThuocOptions = await BuildNhomThuocOptionsAsync(model.MedicineCategoryId, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Cập nhật thông tin thuốc thành công!";
        return RedirectToAction(nameof(Index), new { area = "Admin" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTonKho(
        int id,
        int stockQuantity,
        string? tuKhoa = null,
        int? medicineCategoryId = null,
        bool? trangThai = null,
        string? trangThaiTonKho = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await _danhMucService.CapNhatTonKhoThuocAsync(id, stockQuantity, cancellationToken);
        if (!result.Success)
        {
            TempData["Error"] = result.Error ?? "Không thể cập nhật tồn kho.";
        }
        else
        {
            TempData["Success"] = $"Đã cập nhật tồn kho thành {stockQuantity:N0}.";
        }

        return RedirectToAction(nameof(Index), new { area = "Admin", tuKhoa, medicineCategoryId, trangThai, trangThaiTonKho, page });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiTrangThai(
        int id,
        bool isActive,
        string? tuKhoa = null,
        int? medicineCategoryId = null,
        bool? trangThai = null,
        string? trangThaiTonKho = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await _danhMucService.DoiTrangThaiThuocAsync(id, isActive, cancellationToken);
        if (!result.Success)
        {
            TempData["Error"] = result.Error ?? "Không thể cập nhật trạng thái thuốc.";
        }
        else
        {
            TempData["Success"] = isActive
                ? "Đã kích hoạt lại thuốc."
                : "Đã ngừng sử dụng thuốc.";
        }

        return RedirectToAction(nameof(Index), new { area = "Admin", tuKhoa, medicineCategoryId, trangThai, trangThaiTonKho, page });
    }

    private async Task<List<SelectListItem>> BuildNhomThuocOptionsAsync(int? selectedId, CancellationToken cancellationToken)
    {
        var categories = await _danhMucService.LayTatCaNhomThuocAsync(cancellationToken);
        return categories
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = selectedId == c.Id
            })
            .ToList();
    }
}
