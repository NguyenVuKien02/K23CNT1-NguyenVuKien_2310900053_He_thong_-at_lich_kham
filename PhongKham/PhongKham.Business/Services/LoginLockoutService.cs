using System.Collections.Concurrent;
using PhongKham.Common.Constants;

namespace PhongKham.Business.Services;

public class LoginLockoutService : ILoginLockoutService
{
    private static readonly TimeSpan LockoutWindow = TimeSpan.FromMinutes(QuyDinh.ThoiGianKhoaDangNhapPhut);
    private readonly ConcurrentDictionary<string, List<DateTime>> _failedAttempts = new(StringComparer.OrdinalIgnoreCase);

    public bool IsLockedOut(string normalizedEmail, out int remainingMinutes)
    {
        remainingMinutes = 0;

        if (!_failedAttempts.TryGetValue(normalizedEmail, out var attempts))
        {
            return false;
        }

        var utcNow = DateTime.UtcNow;
        lock (attempts)
        {
            PruneExpired(attempts, utcNow);

            if (attempts.Count < QuyDinh.SoLanDangNhapSaiToiDa)
            {
                return false;
            }

            var unlockAt = attempts[0].Add(LockoutWindow);
            var remaining = unlockAt - utcNow;
            remainingMinutes = Math.Max(1, (int)Math.Ceiling(remaining.TotalMinutes));
            return true;
        }
    }

    public void RecordFailedAttempt(string normalizedEmail)
    {
        var attempts = _failedAttempts.GetOrAdd(normalizedEmail, static _ => []);
        var utcNow = DateTime.UtcNow;

        lock (attempts)
        {
            PruneExpired(attempts, utcNow);
            attempts.Add(utcNow);
        }
    }

    public void ResetAttempts(string normalizedEmail)
    {
        _failedAttempts.TryRemove(normalizedEmail, out _);
    }

    private static void PruneExpired(List<DateTime> attempts, DateTime utcNow)
    {
        var cutoff = utcNow.Subtract(LockoutWindow);
        attempts.RemoveAll(t => t <= cutoff);
    }
}
