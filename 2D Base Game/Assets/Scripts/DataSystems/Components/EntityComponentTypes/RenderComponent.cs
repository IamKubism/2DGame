using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;

namespace Psingine
{
    [JsonObject(MemberSerialization.OptIn), Serializable]
    public class RenderComponent : IBaseComponent
    {
        [JsonProperty]
        public string layer_name;

        [JsonProperty]
        public string sprite_name;

        public SubscriberEvent subscriber { get; set; }


        public RenderComponent()
        {
        }

        public RenderComponent(RenderComponent r)
        {
            sprite_name = r.sprite_name;
            layer_name = r.layer_name;
        }

        public RenderComponent(string layer_name, string sprite_name, RenderComponent r)
        {
            this.layer_name = layer_name;
            this.sprite_name = sprite_name;
        }

        public RenderComponent(JObject obj)
        {
            layer_name = sprite_name = "NULL";
            if (obj["layer_name"] != null)
            {
                layer_name = obj.Value<string>("layer_name");
            }
            if(obj["sprite_name"] != null)
            {
                sprite_name = obj.Value<string>("sprite_name");
            }
        }

        public override string ToString()
        {
            return $"sprite: {sprite_name} layer: {layer_name}";
        }

        public void SetStateValues(object vals)
        {
            RenderComponent over = (RenderComponent)vals;
            sprite_name = over.sprite_name;
            layer_name = over.layer_name;
        }

        public void SetListener(SubscriberEvent subscriber)
        {
            this.subscriber = subscriber;
        }

        public bool Trigger(Event e)
        {
            return true;
        }

        public bool SetSubscriberListener(Action<IBaseComponent> action, bool before_after)
        {
            throw new NotImplementedException();
        }

        public void CopyData(IBaseComponent comp)
        {
            throw new NotImplementedException();
        }
    }
}



