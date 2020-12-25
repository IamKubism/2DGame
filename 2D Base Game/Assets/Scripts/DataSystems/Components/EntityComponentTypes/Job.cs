using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Psingine
{
    public class TimePassInvoke : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }
        FloatMinMax progress_remaining;
        Event done_invoke;

        public TimePassInvoke()
        {
            done_invoke = Event.NewEvent("NullEvent");
            progress_remaining = new FloatMinMax();
        }

        public TimePassInvoke(TimePassInvoke prot)
        {
            progress_remaining = new FloatMinMax(prot.progress_remaining);
            done_invoke = new Event(prot.done_invoke);
        }

        public void CopyData(IBaseComponent comp)
        {
            progress_remaining = new FloatMinMax(((TimePassInvoke)comp).progress_remaining);
        }

        public bool Trigger(Event e)
        {
            if (e.HasTag(BaseEvents.pass_time))
            {
                e.AddUpdate(AdvanceProgress, 100);
                e.AddUpdate(SetProgress, 10);
            }
            return true;
        }

        void AdvanceProgress(Event e)
        {
            subscriber.OperateBeforeOnComp();
            progress_remaining += e.GetParamValue<float>(EventParams.job_progress);
            subscriber.OperateAfterOnComp();
        }

        void SetProgress(Event e)
        {
            subscriber.OperateBeforeOnComp();
            e.SetParamValue(EventParams.job_progress, e.GetParamValue<float>(EventParams.time_dt), (o1, o2) => { return o2; });
            subscriber.OperateAfterOnComp();

        }

        void CompleteJob(Event e)
        {
            subscriber.OperateBeforeOnComp();
            done_invoke.Invoke(e.GetParamValue<Entity>(EventParams.invoking_entity));
            subscriber.OperateAfterOnComp();
        }
    }
}

