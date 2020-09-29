using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SelectionComponent : IBaseComponent
    {
        [JsonProperty]
        public int priority;

        SubscriberEvent<SelectionComponent> subscriber;

        public SelectionComponent()
        {
        }

        public SelectionComponent(SelectionComponent s)
        {
            priority = s.priority;
        }

        public void SetListener<T>(SubscriberEvent<T> subscriber) where T : IBaseComponent
        {
            if (typeof(T) != this.GetType())
            {
                Debug.LogError("Could not set base statistic subscriber, wrong subscriber type");
            }
            else
            {
                this.subscriber = (SubscriberEvent<SelectionComponent>)Convert.ChangeType(subscriber, typeof(SubscriberEvent<SelectionComponent>));
            }
        }
    }
}

