using System.Collections.Generic;

namespace Services.Response
{
    public class NumberOfLevelResponse
    {
        private List<LevelTypeCount> Values { get; set; }
    }
    
    public class LevelTypeCount
    {
        private GameMode Mode { get; set; }
        private int Count { get; set; }
    }

}