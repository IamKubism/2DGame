using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public class BehaviorComponent : IBaseComponent
    {
        string behavior_type;
        public IBehavior behavior;

        public BehaviorComponent(string behavior_type)
        {
            this.behavior_type = behavior_type;
        }


    }
}

