using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace HighKings
{
    public class Walker : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }
        public float speed;

        public Walker()
        {
            speed = 1f;
        }

        public Walker(JObject obj)
        {
            speed = 1f;
            if(obj["speed"] != null)
            {
                speed = obj.Value<float>("speed");
            }
        }

        public Walker(Walker w)
        {
            speed = w.speed;
        }

        public bool Trigger(Event e)
        {
            bool eval = true;
            if (e.tags.Contains("Preference") && e.tags.Contains("Movement"))
            {
                e.AddUpdate(SetMoveCost, 15);
            }
            return eval;
        }

        public void SetMoveCost(Event e)
        {
            float terrain = e.GetParamValue<float>("terrain_cost");
            e.SetParamValue("move_cost", terrain/speed, MathFunctions.NegativeRespectingSum);
        }
    }

}
