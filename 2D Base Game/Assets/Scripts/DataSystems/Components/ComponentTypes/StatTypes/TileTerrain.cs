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

        SubscriberEvent<TileTerrain> subscriber;

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

        public void SetListener<T>(SubscriberEvent<T> subscriber) where T : IBaseComponent
        {
            if (typeof(T) != this.GetType())
            {
                Debug.LogError("Could not set base statistic subscriber, wrong subscriber type");
            }
            else
            {
                this.subscriber = (SubscriberEvent<TileTerrain>)Convert.ChangeType(subscriber, typeof(SubscriberEvent<TileTerrain>));
            }
        }

        public bool Trigger(Event e)
        {
            throw new System.NotImplementedException();
        }
    }
}
