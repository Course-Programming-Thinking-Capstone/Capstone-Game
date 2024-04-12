using System.Collections.Generic;

namespace Services.Response
{

    public class UserProcessResponse
    {
        public GameMode mode { get; set; }
        public List<int> PlayedLevel { get; set; }
    }

   
}