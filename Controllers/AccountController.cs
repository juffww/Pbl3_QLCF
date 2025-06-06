using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using pbl3_QLCF.Data;
using System.Text.RegularExpressions;

// Trước tiên, cần cài đặt package BCrypt.Net-Next
// Trong Package Manager Console chạy: Install-Package BCrypt.Net-Next

public class AccountController : Controller
{
    private readonly Pbl3Context _context;

    public AccountController(Pbl3Context context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult DoiMatKhau(string MatKhauCu, string MatKhauMoi, string XacNhanMatKhau)
    {
        try
        {
            // Lấy thông tin user hiện tại (từ session hoặc claim)
            var userId = HttpContext.Session.GetString("maNV");
            // Hoặc có thể lấy từ User.Identity nếu dùng Authentication
            // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn" });
            }

            var user = _context.NguoiDungs.Find(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin người dùng" });
            }

            // Kiểm tra mật khẩu cũ
            if (!VerifyPassword(MatKhauCu, user.MatKhau))
            {
                return Json(new
                {
                    success = false,
                    errors = new { MatKhauCu = "Mật khẩu hiện tại không chính xác" }
                });
            }

            // Validate mật khẩu mới
            var validationResult = ValidateNewPassword(MatKhauMoi, XacNhanMatKhau);
            if (!validationResult.IsValid)
            {
                return Json(new { success = false, errors = validationResult.Errors });
            }

            // Kiểm tra mật khẩu mới không được trùng với mật khẩu cũ
            if (VerifyPassword(MatKhauMoi, user.MatKhau))
            {
                return Json(new
                {
                    success = false,
                    errors = new { MatKhauMoi = "Mật khẩu mới không được trùng với mật khẩu hiện tại" }
                });
            }

            // Cập nhật mật khẩu mới
            user.MatKhau = HashPassword(MatKhauMoi);
            _context.Update(user);
            _context.SaveChanges();

            // Log hoạt động đổi mật khẩu (tùy chọn)
            LogPasswordChange(userId);

            return Json(new { success = true, message = "Đổi mật khẩu thành công" });
        }
        catch (Exception ex)
        {
            // Log lỗi
            // _logger.LogError(ex, "Error changing password for user {UserId}", userId);

            return Json(new { success = false, message = "Lỗi hệ thống, vui lòng thử lại sau" });
        }
    }

    /// <summary>
    /// Mã hóa mật khẩu sử dụng BCrypt
    /// </summary>
    /// <param name="password">Mật khẩu plaintext</param>
    /// <returns>Mật khẩu đã được mã hóa</returns>
    private string HashPassword(string password)
    {
        // Tạo salt và hash mật khẩu
        // workFactor = 12 là mức độ bảo mật cao (khuyến nghị từ 10-15)
        // Số càng cao thì càng an toàn nhưng xử lý càng chậm
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    /// <summary>
    /// Xác minh mật khẩu
    /// </summary>
    /// <param name="password">Mật khẩu plaintext</param>
    /// <param name="hashedPassword">Mật khẩu đã mã hóa</param>
    /// <returns>True nếu mật khẩu khớp</returns>
    private bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            // BCrypt sẽ tự động xử lý salt và kiểm tra
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch (Exception)
        {
            // Nếu hashedPassword không đúng định dạng BCrypt
            return false;
        }
    }

    /// <summary>
    /// Validate mật khẩu mới theo các tiêu chí bảo mật
    /// </summary>
    /// <param name="newPassword">Mật khẩu mới</param>
    /// <param name="confirmPassword">Xác nhận mật khẩu</param>
    /// <returns>Kết quả validation</returns>
    private ValidationResult ValidateNewPassword(string newPassword, string confirmPassword)
    {
        var result = new ValidationResult { IsValid = true, Errors = new Dictionary<string, string>() };

        // Kiểm tra mật khẩu không được rỗng
        if (string.IsNullOrWhiteSpace(newPassword))
        {
            result.Errors["MatKhauMoi"] = "Mật khẩu mới không được để trống";
            result.IsValid = false;
        }
        else
        {
            // Kiểm tra độ dài tối thiểu
            if (newPassword.Length < 6)
            {
                result.Errors["MatKhauMoi"] = "Mật khẩu phải có ít nhất 6 ký tự";
                result.IsValid = false;
            }
            else if (newPassword.Length > 100)
            {
                result.Errors["MatKhauMoi"] = "Mật khẩu không được vượt quá 100 ký tự";
                result.IsValid = false;
            }

            // Kiểm tra độ mạnh của mật khẩu
            if (!IsStrongPassword(newPassword))
            {
                result.Errors["MatKhauMoi"] = "Mật khẩu phải chứa ít nhất 1 chữ hoa, 1 chữ thường và 1 số";
                result.IsValid = false;
            }

            // Kiểm tra các pattern không an toàn
            if (HasCommonPattern(newPassword))
            {
                result.Errors["MatKhauMoi"] = "Mật khẩu quá đơn giản, vui lòng chọn mật khẩu khác";
                result.IsValid = false;
            }
        }

        // Kiểm tra xác nhận mật khẩu
        if (string.IsNullOrWhiteSpace(confirmPassword))
        {
            result.Errors["XacNhanMatKhau"] = "Vui lòng xác nhận mật khẩu mới";
            result.IsValid = false;
        }
        else if (newPassword != confirmPassword)
        {
            result.Errors["XacNhanMatKhau"] = "Mật khẩu xác nhận không khớp";
            result.IsValid = false;
        }

        return result;
    }

    /// <summary>
    /// Kiểm tra độ mạnh của mật khẩu
    /// </summary>
    /// <param name="password">Mật khẩu cần kiểm tra</param>
    /// <returns>True nếu mật khẩu đủ mạnh</returns>
    private bool IsStrongPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            return false;

        // Kiểm tra có ít nhất 1 chữ hoa
        bool hasUpper = password.Any(char.IsUpper);

        // Kiểm tra có ít nhất 1 chữ thường
        bool hasLower = password.Any(char.IsLower);

        // Kiểm tra có ít nhất 1 số
        bool hasNumber = password.Any(char.IsDigit);

        // Tùy chọn: kiểm tra ký tự đặc biệt
        // bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        return hasUpper && hasLower && hasNumber;
    }

    /// <summary>
    /// Kiểm tra mật khẩu có chứa các pattern phổ biến không an toàn
    /// </summary>
    /// <param name="password">Mật khẩu cần kiểm tra</param>
    /// <returns>True nếu chứa pattern không an toàn</returns>
    private bool HasCommonPattern(string password)
    {
        var lowerPassword = password.ToLower();

        // Danh sách các pattern phổ biến cần tránh
        var commonPatterns = new[]
        {
            "123456", "password", "123123", "admin", "user",
            "qwerty", "abc123", "111111", "123321", "password123",
            "admin123", "root", "test", "guest", "welcome"
        };

        // Kiểm tra password có chứa pattern phổ biến
        if (commonPatterns.Any(pattern => lowerPassword.Contains(pattern)))
            return true;

        // Kiểm tra chuỗi số liên tiếp (123456, 654321)
        if (Regex.IsMatch(password, @"(012|123|234|345|456|567|678|789|890|987|876|765|654|543|432|321|210)"))
            return true;

        // Kiểm tra ký tự lặp (111111, aaaaaa)
        if (Regex.IsMatch(password, @"(.)\1{3,}"))
            return true;

        return false;
    }

    /// <summary>
    /// Log hoạt động đổi mật khẩu (tùy chọn)
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    private void LogPasswordChange(string userId)
    {
        try
        {
            // Có thể lưu vào bảng audit log
            var logEntry = new AuditLog
            {
                UserId = userId,
                Action = "PASSWORD_CHANGED",
                Timestamp = DateTime.UtcNow,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            };

            // _context.AuditLogs.Add(logEntry);
            // _context.SaveChanges();
        }
        catch (Exception ex)
        {
            // Log lỗi nhưng không throw để không ảnh hưởng đến việc đổi mật khẩu
            // _logger.LogWarning(ex, "Failed to log password change for user {UserId}", userId);
        }
    }

    /// <summary>
    /// Tạo mật khẩu ngẫu nhiên mạnh (để reset password)
    /// </summary>
    /// <param name="length">Độ dài mật khẩu (mặc định 12)</param>
    /// <returns>Mật khẩu ngẫu nhiên</returns>
    public static string GenerateRandomPassword(int length = 12)
    {
        const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        const string numbers = "0123456789";
        const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        var allChars = upperCase + lowerCase + numbers + specialChars;
        var random = new Random();
        var password = new char[length];

        // Đảm bảo có ít nhất 1 ký tự từ mỗi loại
        password[0] = upperCase[random.Next(upperCase.Length)];
        password[1] = lowerCase[random.Next(lowerCase.Length)];
        password[2] = numbers[random.Next(numbers.Length)];
        password[3] = specialChars[random.Next(specialChars.Length)];

        // Fill phần còn lại
        for (int i = 4; i < length; i++)
        {
            password[i] = allChars[random.Next(allChars.Length)];
        }

        // Shuffle array để tránh pattern cố định
        for (int i = 0; i < length; i++)
        {
            int j = random.Next(i, length);
            (password[i], password[j]) = (password[j], password[i]);
        }

        return new string(password);
    }
}

/// <summary>
/// Class hỗ trợ kết quả validation
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public Dictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
}

/// <summary>
/// Model cho audit log (tùy chọn)
/// </summary>
public class AuditLog
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Action { get; set; }
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
}

// Extension methods hữu ích cho password
public static class PasswordExtensions
{
    /// <summary>
    /// Kiểm tra mật khẩu có cần được rehash không (khi nâng cấp workFactor)
    /// </summary>
    public static bool NeedsRehash(this string hashedPassword, int workFactor = 12)
    {
        try
        {
            return BCrypt.Net.BCrypt.PasswordNeedsRehash(hashedPassword, workFactor);
        }
        catch
        {
            return true; // Nếu không parse được thì cần rehash
        }
    }

    /// <summary>
    /// Tính điểm độ mạnh của mật khẩu (0-100)
    /// </summary>
    public static int CalculateStrengthScore(this string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return 0;

        int score = 0;

        // Điểm cho độ dài
        if (password.Length >= 8) score += 20;
        else if (password.Length >= 6) score += 10;

        // Điểm cho chữ thường
        if (password.Any(char.IsLower)) score += 20;

        // Điểm cho chữ hoa
        if (password.Any(char.IsUpper)) score += 20;

        // Điểm cho số
        if (password.Any(char.IsDigit)) score += 20;

        // Điểm cho ký tự đặc biệt
        if (password.Any(ch => !char.IsLetterOrDigit(ch))) score += 20;

        // Trừ điểm nếu có pattern lặp
        if (Regex.IsMatch(password, @"(.)\1{2,}")) score -= 10;

        return Math.Max(0, Math.Min(100, score));
    }
}