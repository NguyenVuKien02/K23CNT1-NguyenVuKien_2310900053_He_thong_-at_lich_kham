using PhongKham.Business.DTOs.TaiKhoan;
using PhongKham.Common.Helpers;
using PhongKham.Common.Results;

namespace PhongKham.Business.Services;

public interface ITaiKhoanService
{
    Task<PagedList<TaiKhoanAdminDto>> TimKiemTaiKhoanAdminAsync(
        TaiKhoanFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<Result<TaiKhoanAdminDto>> LayTaiKhoanTheoIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result> DoiTrangThaiTaiKhoanAsync(
        int userId,
        bool isActive,
        int currentAdminUserId,
        CancellationToken cancellationToken = default);

    Task<Result> DatLaiMatKhauAsync(
        DatLaiMatKhauAdminDto dto,
        CancellationToken cancellationToken = default);
}
