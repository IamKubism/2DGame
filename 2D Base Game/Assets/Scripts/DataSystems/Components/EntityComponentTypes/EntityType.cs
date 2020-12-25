using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Psingine
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EntityType : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }

        [JsonProperty]
        public string type_name;

        public EntityType()
        {
            type_name = "default";
        }

        public EntityType(string type_name)
        {
            this.type_name = type_name;
        }

        public EntityType(JProperty prop)
        {
            type_name = prop.Value.Value<string>("type_name");
        }

        public EntityType(EntityType type)
        {
            type_name = type.type_name;
        }

        public void SetTypeName(string type_name, bool call_listeners)
        {
            if (call_listeners)
                subscriber.OperateBeforeOnComp();
            this.type_name = type_name;
            if (call_listeners)
                subscriber.OperateAfterOnComp();
        }

        public bool Trigger(Event e)
        {
            return true;
        }

        public void CopyData(IBaseComponent comp)
        {
            throw new NotImplementedException();
        }
    }
}
