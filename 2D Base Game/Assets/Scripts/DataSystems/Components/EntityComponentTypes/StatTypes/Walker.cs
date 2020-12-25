using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Psingine
{
    public class Walker : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }
        public float speed = 1f;
        public bool walking;

        public Walker()
        {
            speed = 1f;
            walking = true;
        }

        public Walker(JObject obj)
        {
            if(obj[nameof(speed)] != null)
            {
                speed = obj.Value<float>(nameof(speed));
            }
            if(obj[nameof(walking)] != null)
            {
                walking = obj.Value<bool>(nameof(walking));
            }

        }

        public Walker(Walker w)
        {
            speed = w.speed;
        }

        public bool Trigger(Event e)
        {
            bool eval = true;
            if (e.tags.Contains(MovementEvents.CalculateMovementCost.ToString()))
            {
                e.AddUpdate(SetMoveCost, 15);
            }
            if (e.tags.Contains(MovementEvents.ComputeMovementProgress.ToString()))
            {
                e.AddUpdate(SetMoveSpeed, 0);
                e.AddUpdate(SetMoveProgress, 50);
            }
            return eval;
        }

        public void SetMoveCost(Event e)
        {
            float terrain = e.GetParamValue<float>(MovementParams.terrain_cost);
            e.SetParamValue(MovementParams.movement_cost, speed > 0 ?  terrain/speed : Mathf.Infinity, MathFunctions.NegativeRespectingSum);
        }

        public void SetMoveProgress(Event e)
        {
            e.SetParamValue(MovementParams.move_progress, e.GetParamValue<float>(MovementParams.speed), (f1, f2) => { return f1 + f2; });
        }

        public void SetMoveSpeed(Event e)
        {
            e.SetParamValue(MovementParams.speed, speed, (f1, f2) => { return f1 + f2; });
            //Debug.Log("set speed");
        }

        public void CopyData(IBaseComponent comp)
        {
            throw new System.NotImplementedException();
        }
    }

}
