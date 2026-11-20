using Microsoft.EntityFrameworkCore;
using PhongKham.Common.Enums;
using PhongKham.Data.Context;
using PhongKham.Data.Entities;

namespace PhongKham.Data.Repositories;

public class LichHenRepository : Repository<LichHen>, ILichHenRepository
{
    public LichHenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsAsync(int doctorId, DateOnly ngayHen, TimeOnly gioBatDau, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(
            lh => lh.DoctorId == doctorId
                && lh.NgayHen == ngayHen
                && lh.GioBatDau == gioBatDau
                && lh.TrangThai < TrangThaiLichHen.DaHuy,
            cancellationToken);
    }

    public async Task<bool> CoTrungGioAsync(int patientId, DateOnly ngayHen, TimeOnly gioBatDau, int? excludeLichHenId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(lh =>
            lh.PatientId == patientId
            && lh.NgayHen == ngayHen
            && lh.GioBatDau == gioBatDau
            && lh.TrangThai < TrangThaiLichHen.DaHuy);

        if (excludeLichHenId.HasValue)
        {
            query = query.Where(lh => lh.Id != excludeLichHenId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LichHen>> GetByDoctorAndNgayAsync(int doctorId, DateOnly ngayHen, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(lh => lh.Patient)
            .Include(lh => lh.Doctor)
                .ThenInclude(d => d.Specialty)
            .Include(lh => lh.LichLamViec)
                .ThenInclude(llv => llv.CaLamViec)
            .Where(lh => lh.DoctorId == doctorId && lh.NgayHen == ngayHen)
            .OrderBy(lh => lh.GioBatDau)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LichHen>> GetByPatientAsync(int patientId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(lh => lh.Doctor)
                .ThenInclude(d => d.Specialty)
            .Include(lh => lh.LichLamViec)
                .ThenInclude(llv => llv.CaLamViec)
            .Include(lh => lh.DanhGium)
            .Include(lh => lh.PhieuKham)
            .Where(lh => lh.PatientId == patientId)
            .OrderByDescending(lh => lh.NgayHen)
            .ThenByDescending(lh => lh.GioBatDau)
            .ToListAsync(cancellationToken);
    }

    public async Task<LichHen?> GetWithChiTietAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(lh => lh.Patient)
                .ThenInclude(p => p.HoSoBenhAn)
            .Include(lh => lh.Doctor)
                .ThenInclude(d => d.Specialty)
            .Include(lh => lh.LichLamViec)
                .ThenInclude(llv => llv.CaLamViec)
            .Include(lh => lh.LichSuLichHens.OrderByDescending(ls => ls.ThoiGian))
                .ThenInclude(ls => ls.NguoiThayDoi)
            .Include(lh => lh.PhieuKham)
            .Include(lh => lh.DanhGium)
            .FirstOrDefaultAsync(lh => lh.Id == id, cancellationToken);
    }
}
