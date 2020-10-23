using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EntityType : IBaseComponent
    {
        SubscriberEvent<EntityType> listener;

        [JsonProperty]
        public string type_name { get; protected set; }

        public EntityType()
        {
            type_name = "Default";
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
                listener.OperateBeforeOnComp();
            this.type_name = type_name;
            if (call_listeners)
                listener.OperateAfterOnComp();
        }

        public void SetListener<T>(SubscriberEvent<T> subscriber) where T : IBaseComponent
        {
            listener = (SubscriberEvent<EntityType>)System.Convert.ChangeType(subscriber, typeof(SubscriberEvent<EntityType>));
        }
    }
}
