namespace NZWalks.Core.DTOs
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; }

        public DateTime AccessTokenExpiriedTime { get; set; }

        // Chuyển đổi UTC sang giờ địa phương khi lấy giá trị và ngược lại khi muốn gửi về database thì phải chuyển sang UTC (giờ quốc tế khác hệ thống)
        //public DateTime AccessTokenExpiriedTime
        //{
        //    get { return accessTokenExpiriedTime.ToLocalTime(); }
        //    set { accessTokenExpiriedTime = value.ToUniversalTime(); }
        //}
        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiriedTime { get; set; }
        // Chuyển đổi UTC sang giờ địa phương khi lấy giá trị và ngược lại khi muốn gửi về database thì phải chuyển sang UTC (giờ quốc tế khác hệ thống)
        //public DateTime RefreshTokenExpiriedTime
        //{
        //    get { return refreshTokenExpiriedTime.ToLocalTime(); }
        //    set { refreshTokenExpiriedTime = value.ToUniversalTime(); }
        //}
    }
}
