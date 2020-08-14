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

        public BehaviorComponent(string behavior_type, IBehavior behavior)
        {
            this.behavior_type = behavior_type;
            this.behavior = behavior;
        }

        public string ComponentType()
        {
            return behavior_type;
        }
        //

        public bool computable()
        {
            return true;
        }

        public void OnUpdateState()
        {
            throw new System.NotImplementedException();
        }

        public void RegisterUpdateAction(Action<IBaseComponent> update)
        {

        }
    }
}

