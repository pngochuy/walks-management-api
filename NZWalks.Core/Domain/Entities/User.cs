using Microsoft.AspNetCore.Identity;

namespace NZWalks.Core.Domain.Entities
{
    public class User : IdentityUser // Khi thay đổi trong Identity phải chạy lại Migration
    {
        public string? RefreshToken { get; set; }

        private DateTime refreshTokenExpiry;

        // Chuyển đổi UTC sang giờ địa phương khi lấy giá trị và ngược lại khi muốn gửi về database thì phải chuyển sang UTC (giờ quốc tế khác hệ thống)
        // phải convert Datetime từ Now qua UtcNow mới lưu vào PosgresSQL được!
        public DateTime RefreshTokenExpiry
        {
            get { return refreshTokenExpiry.ToLocalTime(); }
            set { refreshTokenExpiry = value.ToUniversalTime(); }
        }
    }
}
