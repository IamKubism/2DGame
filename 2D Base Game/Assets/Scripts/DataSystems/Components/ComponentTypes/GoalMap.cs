using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public class GoalMap : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }

        public bool SetSubscriberListener(Action<IBaseComponent> action, bool before_after)
        {
            throw new NotImplementedException();
        }

        public bool Trigger(Event e)
        {
            return true;
        }
    }

}
