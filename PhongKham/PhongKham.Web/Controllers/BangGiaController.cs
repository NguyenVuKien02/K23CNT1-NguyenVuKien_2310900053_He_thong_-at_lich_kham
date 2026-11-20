using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhongKham.Business.Services;
using PhongKham.Common.Enums;
using PhongKham.Common.Helpers;
using PhongKham.Web.ViewModels.Public;

namespace PhongKham.Web.Controllers;

[AllowAnonymous]
public class BangGiaController : Controller
{
    private readonly IDanhMucService _danhMucService;

    public BangGiaController(IDanhMucService danhMucService)
    {
        _danhMucService = danhMucService;
    }

    [HttpGet("bang-gia")]
    public async Task<IActionResult> Index(
        LoaiDichVu? loaiDichVu = null,
        string? tuKhoa = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var pagedServices = await _danhMucService.LayBangGiaDichVuAsync(
            loaiDichVu,
            tuKhoa,
            page,
            cancellationToken: cancellationToken);

        var items = pagedServices.Items
            .Select(dv => new DichVuItemViewModel
            {
                Id = dv.Id,
                MaDichVu = dv.MaDichVu,
                TenDichVu = dv.TenDichVu,
                LoaiDichVu = dv.LoaiDichVu,
                DonGia = dv.DonGia,
                MoTa = dv.MoTa
            })
            .ToList();

        var viewModel = new BangGiaIndexViewModel
        {
            LoaiDichVu = loaiDichVu,
            TuKhoa = tuKhoa?.Trim(),
            DanhSachDichVu = new PagedList<DichVuItemViewModel>(
                items,
                pagedServices.TotalCount,
                pagedServices.PageIndex,
                pagedServices.PageSize)
        };

        return View(viewModel);
    }
}
