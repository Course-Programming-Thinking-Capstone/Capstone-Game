namespace Services.Response
{
    public class GameShopItemResponse
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = null!;
        public string Details { get; set; } = null!;
        public string SpritesUrl { get; set; } = null!;
        public int ItemRateType { get; set; }
        public int ItemType { get; set; }
        public int Price { get; set; }
    }
}