using PhongKham.Business.DTOs.DanhMuc;
using PhongKham.Common.Enums;
using PhongKham.Common.Helpers;
using PhongKham.Common.Results;

namespace PhongKham.Business.Services;

public interface IDanhMucService
{
    // Public
    Task<IReadOnlyList<ChuyenKhoaPublicDto>> LayChuyenKhoaHoatDongAsync(
        string? tuKhoa = null,
        CancellationToken cancellationToken = default);

    Task<ChuyenKhoaPublicDto?> LayChuyenKhoaTheoIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<PagedList<DichVuPublicDto>> LayBangGiaDichVuAsync(
        LoaiDichVu? loaiDichVu = null,
        string? tuKhoa = null,
        int page = 1,
        int pageSize = PagedList<DichVuPublicDto>.DefaultPageSize,
        CancellationToken cancellationToken = default);

    Task<int> DemDichVuHoatDongAsync(CancellationToken cancellationToken = default);

    // Admin: Chuyên khoa
    Task<PagedList<ChuyenKhoaAdminDto>> TimKiemChuyenKhoaAdminAsync(
        string? tuKhoa = null,
        bool? trangThai = null,
        int page = 1,
        int pageSize = PagedList<ChuyenKhoaAdminDto>.DefaultPageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChuyenKhoaAdminDto>> LayTatCaChuyenKhoaAsync(
        bool chiHoatDong = false,
        CancellationToken cancellationToken = default);

    Task<Result<ChuyenKhoaAdminDto>> LayChuyenKhoaAdminTheoIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result<int>> TaoChuyenKhoaAsync(
        ChuyenKhoaUpsertDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> CapNhatChuyenKhoaAsync(
        ChuyenKhoaUpsertDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> DoiTrangThaiChuyenKhoaAsync(
        int id,
        bool isActive,
        CancellationToken cancellationToken = default);

    // Admin: Nhóm thuốc
    Task<PagedList<NhomThuocAdminDto>> TimKiemNhomThuocAdminAsync(
        string? tuKhoa = null,
        int page = 1,
        int pageSize = PagedList<NhomThuocAdminDto>.DefaultPageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NhomThuocOptionDto>> LayTatCaNhomThuocAsync(
        CancellationToken cancellationToken = default);

    Task<Result<NhomThuocAdminDto>> LayNhomThuocTheoIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result<int>> TaoNhomThuocAsync(
        NhomThuocUpsertDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> CapNhatNhomThuocAsync(
        NhomThuocUpsertDto dto,
        CancellationToken cancellationToken = default);

    // Admin: Thuốc
    Task<PagedList<ThuocAdminDto>> TimKiemThuocAdminAsync(
        ThuocFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<Result<ThuocAdminDto>> LayThuocTheoIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result<int>> TaoThuocAsync(
        ThuocUpsertDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> CapNhatThuocAsync(
        ThuocUpsertDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> CapNhatTonKhoThuocAsync(
        int id,
        int stockQuantity,
        CancellationToken cancellationToken = default);

    Task<Result> DoiTrangThaiThuocAsync(
        int id,
        bool isActive,
        CancellationToken cancellationToken = default);

    // Admin: Dịch vụ
    Task<PagedList<DichVuAdminDto>> TimKiemDichVuAdminAsync(
        DichVuFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<Result<DichVuAdminDto>> LayDichVuTheoIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result<int>> TaoDichVuAsync(
        DichVuUpsertDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> CapNhatDichVuAsync(
        DichVuUpsertDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> DoiTrangThaiDichVuAsync(
        int id,
        bool conHieuLuc,
        CancellationToken cancellationToken = default);

    // Admin: Ca làm việc
    Task<PagedList<CaLamViecAdminDto>> TimKiemCaLamViecAdminAsync(
        string? tuKhoa = null,
        int page = 1,
        int pageSize = PagedList<CaLamViecAdminDto>.DefaultPageSize,
        CancellationToken cancellationToken = default);

    Task<Result<CaLamViecAdminDto>> LayCaLamViecTheoIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result<int>> TaoCaLamViecAsync(
        CaLamViecUpsertDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> CapNhatCaLamViecAsync(
        CaLamViecUpsertDto dto,
        CancellationToken cancellationToken = default);
}
