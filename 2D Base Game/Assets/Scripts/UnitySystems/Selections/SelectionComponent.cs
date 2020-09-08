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

        public Dictionary<string, Func<Entity, string>> string_generators;

        public SelectionComponent()
        {
            string_generators = new Dictionary<string, Func<Entity, string>>();
        }

        public SelectionComponent(SelectionComponent s)
        {
            priority = s.priority;
            string_generators = new Dictionary<string, Func<Entity, string>>(s.string_generators);
        }
    }
}

