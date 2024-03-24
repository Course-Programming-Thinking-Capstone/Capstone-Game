using System.Collections.Generic;
using Newtonsoft.Json;

namespace Services.Response
{

    public class UserProcessResponse
    {
        public GameMode mode { get; set; }
        public int levelIndex { get; set; }
    }

   
}