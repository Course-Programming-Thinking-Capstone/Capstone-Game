using System.Collections.Generic;

namespace Services.Response
{
    public class BuyResponse
    {
        public int CurrentCoin { get; set; }
        public int CurrentGem { get; set; }
        public List<int> OwnedItem { get; set; } = null!;
    }
}