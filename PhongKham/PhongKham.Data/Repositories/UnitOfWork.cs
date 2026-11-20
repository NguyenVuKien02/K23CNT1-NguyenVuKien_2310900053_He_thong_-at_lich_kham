using Microsoft.EntityFrameworkCore.Storage;
using PhongKham.Data.Context;

namespace PhongKham.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _currentTransaction;

    public ILichHenRepository LichHens { get; }

    public IPhieuKhamRepository PhieuKhams { get; }

    public IHoaDonRepository HoaDons { get; }

    public UnitOfWork(
        ApplicationDbContext context,
        ILichHenRepository lichHens,
        IPhieuKhamRepository phieuKhams,
        IHoaDonRepository hoaDons)
    {
        _context = context;
        LichHens = lichHens;
        PhieuKhams = phieuKhams;
        HoaDons = hoaDons;
    }

    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        if (_repositories.TryGetValue(type, out var existing))
        {
            return (IRepository<T>)existing;
        }

        var repository = new Repository<T>(_context);
        _repositories[type] = repository;
        return repository;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
        {
            return;
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
        {
            return;
        }

        try
        {
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
        {
            return;
        }

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _currentTransaction = null;
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_currentTransaction is not null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        GC.SuppressFinalize(this);
    }
}
