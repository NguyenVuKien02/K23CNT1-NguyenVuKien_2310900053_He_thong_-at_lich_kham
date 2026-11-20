using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhongKham.Business.Services;
using PhongKham.Web.Models;
using PhongKham.Web.ViewModels.Public;

namespace PhongKham.Web.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly IDanhMucService _danhMucService;
    private readonly IDoctorService _doctorService;

    public HomeController(IDanhMucService danhMucService, IDoctorService doctorService)
    {
        _danhMucService = danhMucService;
        _doctorService = doctorService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var chuyenKhoas = await _danhMucService.LayChuyenKhoaHoatDongAsync(cancellationToken: cancellationToken);
        var bacSiNoiBat = await _doctorService.LayBacSiNoiBatAsync(6, cancellationToken);
        var goiYBacSi = await _doctorService.LayDanhSachGoiYBacSiAsync(cancellationToken);
        var tongBacSi = await _doctorService.DemBacSiHoatDongAsync(cancellationToken);
        var tongDichVu = await _danhMucService.DemDichVuHoatDongAsync(cancellationToken);

        var chuyenKhoaVms = chuyenKhoas
            .Select(c => new ChuyenKhoaCardViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                SoBacSi = c.SoBacSi
            })
            .ToList();

        var viewModel = new HomeIndexViewModel
        {
            ChuyenKhoas = chuyenKhoaVms,
            ChuyenKhoaNoiBat = chuyenKhoaVms.Take(6).ToList(),
            BacSiNoiBat = bacSiNoiBat
                .Select(b => new BacSiCardViewModel
                {
                    Id = b.Id,
                    SpecialtyId = b.SpecialtyId,
                    SpecialtyName = b.SpecialtyName,
                    FullName = b.FullName,
                    Degree = b.Degree,
                    ExperienceYears = b.ExperienceYears,
                    Bio = b.Bio,
                    AvatarUrl = b.AvatarUrl,
                    ConsultationFee = b.ConsultationFee,
                    DiemDanhGiaTrungBinh = b.DiemDanhGiaTrungBinh,
                    SoLuotDanhGia = b.SoLuotDanhGia
                })
                .ToList(),
            GoiYBacSi = goiYBacSi
                .Select(b => new BacSiGoiYViewModel
                {
                    Id = b.Id,
                    SpecialtyId = b.SpecialtyId,
                    FullName = b.FullName,
                    Degree = b.Degree
                })
                .ToList(),
            TongChuyenKhoa = chuyenKhoaVms.Count,
            TongBacSi = tongBacSi,
            TongDichVu = tongDichVu,
            NgayMacDinh = DateOnly.FromDateTime(DateTime.Today)
        };

        return View(viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
