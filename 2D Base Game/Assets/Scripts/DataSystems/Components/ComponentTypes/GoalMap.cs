using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public class GoalMap : IBaseComponent
    {
        SubscriberEvent<GoalMap> listener;

        public void SetListener<T>(SubscriberEvent<T> subscriber) where T : IBaseComponent
        {
            listener = (SubscriberEvent<GoalMap>)Convert.ChangeType(subscriber, typeof(SubscriberEvent<GoalMap>));
        }

        public bool Trigger(Event e)
        {
            throw new System.NotImplementedException();
        }
    }

}
