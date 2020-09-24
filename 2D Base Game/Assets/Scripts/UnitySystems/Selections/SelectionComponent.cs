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

        public SelectionComponent()
        {
        }

        public SelectionComponent(SelectionComponent s)
        {
            priority = s.priority;
        }
    }
}

