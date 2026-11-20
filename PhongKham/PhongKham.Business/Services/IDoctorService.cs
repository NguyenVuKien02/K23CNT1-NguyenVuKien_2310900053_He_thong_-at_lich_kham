using PhongKham.Business.DTOs.Doctors;
using PhongKham.Common.Helpers;
using PhongKham.Common.Results;

namespace PhongKham.Business.Services;

public interface IDoctorService
{
    // Public
    Task<IReadOnlyList<BacSiPublicDto>> LayBacSiNoiBatAsync(
        int soLuong = 6,
        CancellationToken cancellationToken = default);

    Task<PagedList<BacSiPublicDto>> TimKiemBacSiPublicAsync(
        BacSiFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<Result<BacSiChiTietDto>> LayChiTietBacSiPublicAsync(
        int id,
        int reviewPage = 1,
        int reviewPageSize = PagedList<DanhGiaBacSiPublicDto>.DefaultPageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BacSiGoiYDto>> LayDanhSachGoiYBacSiAsync(
        CancellationToken cancellationToken = default);

    Task<int> DemBacSiHoatDongAsync(CancellationToken cancellationToken = default);

    // Admin
    Task<PagedList<BacSiAdminDto>> TimKiemBacSiAdminAsync(
        BacSiAdminFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<Result<BacSiAdminDto>> LayBacSiAdminTheoIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result<int>> TaoBacSiAsync(
        BacSiCreateDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> CapNhatBacSiAsync(
        BacSiUpdateDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> DoiTrangThaiBacSiAsync(
        int id,
        bool isActive,
        CancellationToken cancellationToken = default);
}
