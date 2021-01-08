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
            string s = $"Terrain Cost: {move_cost}";
            return s;
        }

        public bool Trigger(Event e)
        {
            bool eval = true;
            if(e.tags.Contains("SetMovementData"))
            {
                e.AddUpdate(SetTerrainCost, 0);
            }
            if (e.tags.Contains("ComputeMovementProgress"))
            {
                e.AddUpdate(SetMovementProgress, 40);
            }
            return eval;
        }

        public void SetTerrainCost(Event e)
        {
            e.SetParamValue("terrain_cost", move_cost, (o1,o2) => { return AddCosts(o1, o2); });
        }

        public void SetMovementProgress(Event e)
        {
            e.SetParamValue("move_progress", move_cost > 0 ? 1 / move_cost : -1f, (o1, o2) => { return MultCosts(o1, o2); });
        }

        public float AddCosts(float f1, float f2)
        {
            return Mathf.Min(Mathf.Sign(f1),Mathf.Sign(f2))*Mathf.Abs(f1 + f2);
        }

        public float MultCosts(float f1, float f2)
        {
            return f1 > 0 && f2 > 0 ? Mathf.Min(Mathf.Sign(f1),Mathf.Sign(f2))*Mathf.Abs(f1*f2) : -1f;
        }
    }
}
