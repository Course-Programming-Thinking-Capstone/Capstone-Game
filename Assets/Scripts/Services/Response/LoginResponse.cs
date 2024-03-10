namespace Services.Response
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public int UserCoin { get; set; }
        public int UserGem { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}