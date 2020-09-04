using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class BaseStatistic : IBaseComponent
    { 
        public string stat_name { get; protected set; }
        public float value;

        public BaseStatistic(string stat_name, float value)
        {
            this.stat_name = stat_name;
            this.value = value;
        }
    }
}

