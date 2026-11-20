using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PhongKham.Common.Constants;
using PhongKham.Common.Results;

namespace PhongKham.Business.Helpers;

internal static class BusinessValidationHelper
{
    private static readonly EmailAddressAttribute EmailValidator = new();

    public static Result KiemTraDoManhMatKhau(string? password)
    {
        if (string.IsNullOrEmpty(password) ||
            password.Length < QuyDinh.DoDaiMatKhauToiThieu ||
            !password.Any(char.IsLetter) ||
            !password.Any(char.IsDigit))
        {
            return Result.Fail(
                $"Mật khẩu phải có ít nhất {QuyDinh.DoDaiMatKhauToiThieu} ký tự, bao gồm cả chữ cái và chữ số.");
        }

        return Result.Ok();
    }

    public static string NormalizeEmail(string? email) =>
        (email ?? string.Empty).Trim().ToLowerInvariant();

    public static bool IsValidEmail(string? email) =>
        !string.IsNullOrWhiteSpace(email) && EmailValidator.IsValid(email);

    public static bool IsUniqueConstraintViolation(DbUpdateException exception) =>
        exception.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627);

    public static string TaoMaTam(string prefix) =>
        $"T{prefix}{Guid.NewGuid():N}"[..20];

    public static string? TrimOrNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public static bool IsSafeUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return true;
        }

        var trimmed = url.Trim();
        return trimmed.StartsWith("/", StringComparison.Ordinal) ||
               trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
               trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase);
    }
}
