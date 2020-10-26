using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class ActiveAttack : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }
        public Entity attack;

        public ActiveAttack()
        {

        }

        public bool Trigger(Event e)
        {
            throw new System.NotImplementedException();
        }
    }
}
