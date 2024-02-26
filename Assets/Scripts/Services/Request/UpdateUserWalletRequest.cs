namespace Services.Request
{
    public class UpdateUserWalletRequest
    {
        public int UserId { get; set; }
        public int Wallet { get; set; }
        public int Coin { get; set; }
    }
}