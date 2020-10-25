using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class FlagComponent : IBaseComponent
    {
        [JsonProperty]
        bool active;

        [JsonProperty]
        string flag_name;

        SubscriberEvent<FlagComponent> subscriber_event;

        public FlagComponent()
        {
            active = false;
            flag_name = "";
        }

        public FlagComponent(FlagComponent f)
        {
            active = f.active;
            flag_name = f.flag_name;
        }

        public FlagComponent(string flag_name, bool active, FlagComponent f)
        {
            this.flag_name = flag_name;
            this.active = active;
        }

        public FlagComponent(bool active, FlagComponent f)
        {
            flag_name = f.flag_name;
            this.active = active;
        }

        public FlagComponent(JProperty prop)
        {
            flag_name = prop.Name;
            active = prop.Value["data"].Value<bool>("active");
        }

        public void SetActiveNoTrigger(bool active)
        {
            this.active = active;
        }

        public void SetActive()
        {
            subscriber_event.OperateBeforeOnComp();
            active = true;
            subscriber_event.OperateAfterOnComp();
        }

        public void SetInactive()
        {
            subscriber_event.OperateBeforeOnComp();
            active = false;
            subscriber_event.OperateAfterOnComp();
        }

        public void ToggleActive()
        {
            subscriber_event.OperateBeforeOnComp();
            active = active ^ true;
            subscriber_event.OperateAfterOnComp();
        }

        public void SetListener<T>(SubscriberEvent<T> subscriber) where T : IBaseComponent
        {
            if (typeof(T) != GetType())
            {
                Debug.LogError("Could not set position subscriber, wrong type");
            }
            else
            {
                this.subscriber_event = (SubscriberEvent<FlagComponent>)Convert.ChangeType(subscriber, typeof(SubscriberEvent<FlagComponent>));
            }
        }

        public bool Trigger(Event e)
        {
            return true;
        }
    }
}

