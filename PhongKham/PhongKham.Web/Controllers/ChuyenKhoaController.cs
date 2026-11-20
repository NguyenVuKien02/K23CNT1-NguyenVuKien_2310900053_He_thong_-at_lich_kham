using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhongKham.Business.Services;
using PhongKham.Web.ViewModels.Public;

namespace PhongKham.Web.Controllers;

[AllowAnonymous]
public class ChuyenKhoaController : Controller
{
    private readonly IDanhMucService _danhMucService;

    public ChuyenKhoaController(IDanhMucService danhMucService)
    {
        _danhMucService = danhMucService;
    }

    [HttpGet("chuyen-khoa")]
    public async Task<IActionResult> Index(string? tuKhoa = null, CancellationToken cancellationToken = default)
    {
        var chuyenKhoas = await _danhMucService.LayChuyenKhoaHoatDongAsync(tuKhoa, cancellationToken);

        var viewModel = new ChuyenKhoaIndexViewModel
        {
            TuKhoa = tuKhoa?.Trim(),
            ChuyenKhoas = chuyenKhoas
                .Select(c => new ChuyenKhoaCardViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    SoBacSi = c.SoBacSi
                })
                .ToList()
        };

        return View(viewModel);
    }
}
