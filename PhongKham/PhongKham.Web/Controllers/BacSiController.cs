using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhongKham.Business.DTOs.Doctors;
using PhongKham.Business.Services;
using PhongKham.Common.Helpers;
using PhongKham.Web.ViewModels.Public;

namespace PhongKham.Web.Controllers;

[AllowAnonymous]
public class BacSiController : Controller
{
    private readonly IDoctorService _doctorService;
    private readonly IDanhMucService _danhMucService;

    public BacSiController(IDoctorService doctorService, IDanhMucService danhMucService)
    {
        _doctorService = doctorService;
        _danhMucService = danhMucService;
    }

    [HttpGet("bac-si")]
    public async Task<IActionResult> Index(
        int? chuyenKhoaId = null,
        string? tuKhoa = null,
        DateOnly? ngayKham = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var chuyenKhoas = await _danhMucService.LayChuyenKhoaHoatDongAsync(cancellationToken: cancellationToken);
        var pagedDoctors = await _doctorService.TimKiemBacSiPublicAsync(
            new BacSiFilterDto
            {
                ChuyenKhoaId = chuyenKhoaId,
                TuKhoa = tuKhoa,
                NgayKham = ngayKham,
                Page = page
            },
            cancellationToken);

        var chuyenKhoaVms = chuyenKhoas
            .Select(c => new ChuyenKhoaCardViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                SoBacSi = c.SoBacSi
            })
            .ToList();

        var doctorVms = pagedDoctors.Items
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
            .ToList();

        var viewModel = new BacSiIndexViewModel
        {
            ChuyenKhoaId = chuyenKhoaId,
            TuKhoa = tuKhoa?.Trim(),
            NgayKham = ngayKham,
            TenChuyenKhoaDangLoc = chuyenKhoaId.HasValue
                ? chuyenKhoaVms.FirstOrDefault(c => c.Id == chuyenKhoaId.Value)?.Name
                : null,
            ChuyenKhoas = chuyenKhoaVms,
            DanhSachBacSi = new PagedList<BacSiCardViewModel>(
                doctorVms,
                pagedDoctors.TotalCount,
                pagedDoctors.PageIndex,
                pagedDoctors.PageSize)
        };

        return View(viewModel);
    }

    [HttpGet("bac-si/{id:int}")]
    public async Task<IActionResult> Details(
        int id,
        int reviewPage = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await _doctorService.LayChiTietBacSiPublicAsync(id, reviewPage, cancellationToken: cancellationToken);
        if (!result.Success || result.Data is null)
        {
            return NotFound();
        }

        var dto = result.Data;
        var reviewVms = dto.DanhGia.Items
            .Select(dg => new DanhGiaBacSiViewModel
            {
                Id = dg.Id,
                TenBenhNhan = dg.TenBenhNhan,
                SoSao = dg.SoSao,
                NhanXet = dg.NhanXet,
                NgayDanhGia = dg.NgayDanhGia
            })
            .ToList();

        var viewModel = new BacSiDetailsViewModel
        {
            Id = dto.Id,
            SpecialtyId = dto.SpecialtyId,
            SpecialtyName = dto.SpecialtyName,
            SpecialtyDescription = dto.SpecialtyDescription,
            FullName = dto.FullName,
            Gender = dto.Gender,
            Degree = dto.Degree,
            ExperienceYears = dto.ExperienceYears,
            Bio = dto.Bio,
            AvatarUrl = dto.AvatarUrl,
            ConsultationFee = dto.ConsultationFee,
            DiemDanhGiaTrungBinh = dto.DiemDanhGiaTrungBinh,
            SoLuotDanhGia = dto.SoLuotDanhGia,
            PhanBoSoSao = dto.PhanBoSoSao,
            LichLamViecSapToi = dto.LichLamViecSapToi
                .Select(l => new LichLamViecBacSiViewModel
                {
                    NgayLam = l.NgayLam,
                    TenCa = l.TenCa,
                    GioBatDau = l.GioBatDau,
                    GioKetThuc = l.GioKetThuc
                })
                .ToList(),
            DanhGia = new PagedList<DanhGiaBacSiViewModel>(
                reviewVms,
                dto.DanhGia.TotalCount,
                dto.DanhGia.PageIndex,
                dto.DanhGia.PageSize)
        };

        return View(viewModel);
    }
}
