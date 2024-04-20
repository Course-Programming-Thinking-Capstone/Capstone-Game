using System.Collections.Generic;

namespace Services.Response
{
    public class UserDataResponse
    {
        public int UserID { get; set; }
        public string DisplayName { get; set; }
        public int OldGem { get; set; }
        public int OldCoin { get; set; }
        public int UserCoin { get; set; }
        public int UserGem { get; set; }
        public List<GameItemResponse> GameItemGet { get; set; }
    }
}