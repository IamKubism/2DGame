using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class ActiveAttack : IBaseComponent
    {
        public SubscriberEvent<ActiveAttack> listener;
        public Entity attack;

        public ActiveAttack()
        {

        }

        public void SetListener<T>(SubscriberEvent<T> subscriber) where T : IBaseComponent
        {
            listener = (SubscriberEvent<ActiveAttack>)Convert.ChangeType(subscriber, typeof(SubscriberEvent<ActiveAttack>));
        }

        public bool Trigger(Event e)
        {
            throw new System.NotImplementedException();
        }
    }
}
