using PhongKham.Business.DTOs.Staffs;
using PhongKham.Common.Helpers;
using PhongKham.Common.Results;

namespace PhongKham.Business.Services;

public interface IStaffService
{
    Task<PagedList<NhanVienAdminDto>> TimKiemNhanVienAdminAsync(
        NhanVienFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<Result<NhanVienAdminDto>> LayNhanVienAdminTheoIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result<int>> TaoNhanVienAsync(
        NhanVienCreateDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> CapNhatNhanVienAsync(
        NhanVienUpdateDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> DoiTrangThaiNhanVienAsync(
        int id,
        bool isActive,
        CancellationToken cancellationToken = default);
}
