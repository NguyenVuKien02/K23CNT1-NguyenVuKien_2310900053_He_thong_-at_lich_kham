namespace PhongKham.Business.Services;

public interface ILoginLockoutService
{
    bool IsLockedOut(string normalizedEmail, out int remainingMinutes);

    void RecordFailedAttempt(string normalizedEmail);

    void ResetAttempts(string normalizedEmail);
}
