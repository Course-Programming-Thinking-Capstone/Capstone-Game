namespace Services.Response
{
    public class LoginResponse
    {
        public string DisplayName { get; set; }
        public string UserCoin { get; set; }
        public string UserGem { get; set; }
        public string Jwt { get; set; }

        public LoginResponse(string displayName, string userCoin, string userGem, string jwt)
        {
            DisplayName = displayName;
            UserCoin = userCoin;
            UserGem = userGem;
            Jwt = jwt;
        }
    }
}