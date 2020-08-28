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

        [JsonProperty]
        public SerializableVector3 position;

        public RenderComponent()
        {
        }

        public RenderComponent(RenderComponent r)
        {
            sprite_name = r.sprite_name;
            layer_name = r.layer_name;
        }

        public RenderComponent(Vector3 position, string layer_name, string sprite_name)
        {
            this.position = position;
            this.layer_name = layer_name;
            this.sprite_name = sprite_name;
        }

        public override string ToString()
        {
            return $"sprite: {sprite_name} layer: {layer_name} position: ({position.ToString()})";
        }

        public void SetStateValues(object vals)
        {
            RenderComponent over = (RenderComponent)vals;
            sprite_name = over.sprite_name;
            layer_name = over.layer_name;
        }
    }
}



