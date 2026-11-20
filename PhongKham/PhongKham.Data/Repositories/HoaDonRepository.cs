using Microsoft.EntityFrameworkCore;
using PhongKham.Common.Enums;
using PhongKham.Data.Context;
using PhongKham.Data.Entities;

namespace PhongKham.Data.Repositories;

public class HoaDonRepository : Repository<HoaDon>, IHoaDonRepository
{
    public HoaDonRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<HoaDon>> GetChuaThanhToanAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(hd => hd.Patient)
            .Include(hd => hd.PhieuKham)
                .ThenInclude(pk => pk.Doctor)
            .Include(hd => hd.ChiTietHoaDons)
            .Where(hd => hd.TrangThai == TrangThaiHoaDon.ChuaThanhToan)
            .OrderByDescending(hd => hd.NgayLap)
            .ToListAsync(cancellationToken);
    }

    public async Task<HoaDon?> GetWithChiTietAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(hd => hd.Patient)
            .Include(hd => hd.NguoiLap)
            .Include(hd => hd.PhieuKham)
                .ThenInclude(pk => pk.Doctor)
                    .ThenInclude(d => d.Specialty)
            .Include(hd => hd.PhieuKham)
                .ThenInclude(pk => pk.LichHen)
            .Include(hd => hd.ChiTietHoaDons)
            .FirstOrDefaultAsync(hd => hd.Id == id, cancellationToken);
    }

    public async Task<HoaDon?> GetByPhieuKhamIdAsync(int phieuKhamId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(hd => hd.Patient)
            .Include(hd => hd.NguoiLap)
            .Include(hd => hd.ChiTietHoaDons)
            .FirstOrDefaultAsync(hd => hd.PhieuKhamId == phieuKhamId, cancellationToken);
    }
}
