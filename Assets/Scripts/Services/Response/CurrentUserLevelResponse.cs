using System.Collections.Generic;
using Newtonsoft.Json;

namespace Services.Response
{

    public class CurrentUserLevelResponse
    {
        private List<CurrentLevelData> Values { get; set; }
    }

    public class CurrentLevelData
    {
        private GameMode Mode { get; set; }
        private int LevelIndex { get; set; }
    }

   
}