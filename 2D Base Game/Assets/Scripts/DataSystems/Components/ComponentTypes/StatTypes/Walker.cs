using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace HighKings
{
    public class Walker : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }
        public float speed = 1f;

        public Walker()
        {
        }

        public Walker(JObject obj)
        {
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
            if (e.tags.Contains("CalculateMovementCost"))
            {
                e.AddUpdate(SetMoveCost, 15);
            }
            if (e.tags.Contains("ComputeMovementProgress"))
            {
                e.AddUpdate(SetMoveProgress, 50);
            }
            return eval;
        }

        public void SetMoveCost(Event e)
        {
            float terrain = e.GetParamValue<float>("terrain_cost");
            e.SetParamValue("move_cost", speed > 0 ? terrain/speed : Mathf.Infinity, MathFunctions.NegativeRespectingSum);
        }

        public void SetMoveProgress(Event e)
        {
            e.SetParamValue("move_progress", speed, (f1, f2) => { return f1 * f2; });
        }
    }

}
