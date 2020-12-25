using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Psingine
{
    public class PhysicalCoolDown : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }
        public FloatMinMax cool_down;

        public PhysicalCoolDown()
        {
            cool_down = new FloatMinMax();
        }

        public PhysicalCoolDown(JProperty prop)
        {
            cool_down = new FloatMinMax();
            //TODO
        }

        public PhysicalCoolDown(PhysicalCoolDown prot)
        {
            cool_down = new FloatMinMax(prot.cool_down);
        }

        public bool Trigger(Event e)
        {
            if (e.tags.Contains(EventTags.DoPhysical.ToString()))
            {
                e.AddUpdate(IncrementAndCancel, 0);
            }

            return true;
        }

        void IncrementAndCancel(Event e)
        {
            if (cool_down.IsOverMax())
            {
                e.GetParamValue<Entity>(EventParams.invoking_entity).RemoveComponent(nameof(PhysicalCoolDown));
            } else
            {
                cool_down += e.GetParamValue<float>(EventParams.time_dt);
                e.Cancel();
            }
        }

        public void CopyData(IBaseComponent comp)
        {
            throw new System.NotImplementedException();
        }
    }
}

