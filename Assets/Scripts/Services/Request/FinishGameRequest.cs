using System;

namespace Services.Request
{
    public class FinishGameRequest
    {
        public int UserID { get; set; }
        public int ModeId { get; set; }
        public int LevelIndex { get; set; }
        public DateTime StartTime { get; set; }
    }
}