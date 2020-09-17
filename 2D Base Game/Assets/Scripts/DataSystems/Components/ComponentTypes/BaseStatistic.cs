using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BaseStatistic : IBaseComponent
    { 
        [JsonProperty]
        public string stat_name { get; protected set; }

        [JsonProperty]
        public int curr_value;

        [JsonProperty]
        public int base_value;

        public BaseStatistic(string stat_name, int curr_value, int base_value)
        {
            this.stat_name = stat_name;
            this.curr_value = curr_value;
            this.base_value = base_value;
        }

        //The clone constructor
        public BaseStatistic(BaseStatistic s)
        {
            stat_name = s.stat_name;
            curr_value = s.curr_value;
            base_value = s.base_value;
        }

        //If this is called its pretty much an error
        public BaseStatistic()
        {
            stat_name = "NULL";
            curr_value = 0;
            base_value = 0;
        }
    }
}

