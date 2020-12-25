using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Psingine
{
    public class AttackTarget : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }
        public string target;

        public AttackTarget(string target)
        {
            this.target = target;
        }

        public bool Trigger(Event e)
        {
            if (e.HasTag(AIStateTestingEvents.Combat))
            {
                e.SetParamValue(CombatParams.AttackTarget, target, (s1,s2) => { return s2; });
            }
            return true;
        }

        public void CopyData(IBaseComponent comp)
        {
            subscriber.OperateBeforeOnComp();
            target = ((AttackTarget)comp).target;
            subscriber.OperateAfterOnComp();
        }
    }
}

