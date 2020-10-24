using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class BluntResistance : IBaseComponent
    {
        SubscriberEvent<BluntResistance> listener;

        public void SetListener<T>(SubscriberEvent<T> subscriber) where T : IBaseComponent
        {
            listener = (SubscriberEvent<BluntResistance>)System.Convert.ChangeType(subscriber, typeof(SubscriberEvent<BluntResistance>));
        }

        public bool Trigger(Event e)
        {
            throw new System.NotImplementedException();
        }
    }

}
