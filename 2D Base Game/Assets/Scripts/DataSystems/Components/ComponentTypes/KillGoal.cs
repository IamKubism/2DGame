using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace HighKings
{
    public class KillGoal : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }
        public Entity target;

        public KillGoal()
        {
        }

        public KillGoal(KillGoal k)
        {
            target = k.target;
        }

        public KillGoal(JObject obj)
        {
            if(obj["target"] != null)
            {
                target = obj.Value<string>("target");
            }
        }

        public bool Trigger(Event e)
        {
            bool eval = true;
            if (target == null)
            {
                eval = false;
                return eval; //TODO: IDK whats goin on with this
            }
            return eval;
        }
    }
}

