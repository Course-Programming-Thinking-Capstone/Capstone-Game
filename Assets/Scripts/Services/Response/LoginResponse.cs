namespace Services.Response
{
    public class LoginResponse
    {
        public int userId { get; set; }
        public string displayName { get; set; }
        public int userCoin { get; set; }
        public int userGem { get; set; }
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
    }
}