﻿using System.Collections;
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

        public SubscriberEvent subscriber { get; set; }


        public SelectionComponent()
        {
        }

        public SelectionComponent(SelectionComponent s)
        {
            priority = s.priority;
        }

        public void SetListener(SubscriberEvent subscriber)
        {
            this.subscriber = subscriber;
        }

        public bool Trigger(Event e)
        {
            return true;
        }
    }
}

