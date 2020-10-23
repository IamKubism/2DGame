using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public abstract class GameComponent<T> : IBaseComponent where T : IBaseComponent
    {
        SubscriberEvent<T> listener;
        public void SetListener<T1>(SubscriberEvent<T1> subscriber) where T1 : IBaseComponent
        {
            listener = (SubscriberEvent<T>)System.Convert.ChangeType(subscriber, typeof(SubscriberEvent<T>));
        }
    }
}
