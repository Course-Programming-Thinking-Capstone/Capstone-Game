namespace Services.Response
{
    public class LevelInformationResponse
    {
        public int coinReward { get; set; }
        public int gameReward { get; set; }
        public int vStartPosition { get; set; }
        public LevelDetail[] levelDetail { get; set; }
    }

    public class LevelDetail
    {
        public int vPosition { get; set; }
        public PositionType positionType { get; set; }
    }
}