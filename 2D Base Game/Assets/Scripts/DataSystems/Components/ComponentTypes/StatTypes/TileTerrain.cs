using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TileTerrain : IBaseComponent
    {
        [JsonProperty]
        public float move_cost;

        public SubscriberEvent subscriber { get; set; }


        [JsonConstructor]
        public TileTerrain(float move_cost = -1f)
        {
            this.move_cost = move_cost;
        }

        public TileTerrain(float move_cost, TileTerrain t)
        {
            this.move_cost = move_cost;
        }

        public TileTerrain(TileTerrain t)
        {
            move_cost = t.move_cost;
        }

        public override string ToString()
        {
            string s = $"Terrain Cost {move_cost}";
            return s;
        }

        public bool Trigger(Event e)
        {
            throw new System.NotImplementedException();
        }

        public bool SetSubscriberListener(Action<IBaseComponent> action, bool before_after)
        {
            throw new NotImplementedException();
        }
    }
}
