namespace Services.Response
{
    public class UserInventoryResponse
    {
        public int Quantity { get; set; }
        public GameItemResponse GameItem { get; set; }
    }
}