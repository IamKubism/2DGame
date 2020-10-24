using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn), Serializable]
    public class RenderComponent : IBaseComponent
    {
        [JsonProperty]
        public string layer_name;

        [JsonProperty]
        public string sprite_name;

        SubscriberEvent<RenderComponent> subscriber;

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

        public void SetListener<T>(SubscriberEvent<T> subscriber) where T : IBaseComponent
        {
            if (typeof(T) != this.GetType())
            {
                Debug.LogError("Could not set base statistic subscriber, wrong subscriber type");
            }
            else
            {
                this.subscriber = (SubscriberEvent<RenderComponent>)Convert.ChangeType(subscriber, typeof(SubscriberEvent<RenderComponent>));
            }
        }

        public bool Trigger(Event e)
        {
            throw new System.NotImplementedException();
        }
    }
}



