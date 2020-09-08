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
        public float value;

        public BaseStatistic(string stat_name, float value)
        {
            this.stat_name = stat_name;
            this.value = value;
        }
    }
}

