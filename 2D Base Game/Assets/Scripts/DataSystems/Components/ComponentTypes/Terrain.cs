using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Terrain : IBaseComponent
    {
        [JsonProperty]
        public float move_cost;

        SubscriberEvent<Terrain> subscriber;

        [JsonConstructor]
        public Terrain(float move_cost = -1f)
        {
            this.move_cost = move_cost;
        }

        public Terrain(Terrain t)
        {
            move_cost = t.move_cost;
        }

        public string ComponentType()
        {
            return "Terrain";
        }

        public bool computable()
        {
            return true;
        }

        public void OnUpdateState()
        {
            throw new System.NotImplementedException();
        }


        public void RegisterUpdateAction(Action<IBaseComponent> update)
        {

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
                this.subscriber = (SubscriberEvent<Terrain>)Convert.ChangeType(subscriber, typeof(SubscriberEvent<Terrain>));
            }
        }
    }
}
