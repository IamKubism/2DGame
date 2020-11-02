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
            bool eval = true;
            if(e.tags.Contains("DataSet") && e.tags.Contains("Tile"))
            {
                e.AddUpdate(SetTerrainCost, 3);
            }
            return eval;
        }

        public bool SetSubscriberListener(Action<IBaseComponent> action, bool before_after)
        {
            throw new NotImplementedException();
        }

        public void SetTerrainCost(Event e)
        {
            e.SetParamValue("terrain_cost", move_cost, (o1,o2) => { return AddCosts(o1, o2); });
        }

        public float AddCosts(float f1, float f2)
        {
            return Mathf.Min(Mathf.Sign(f1),Mathf.Sign(f2))*Mathf.Abs(f1 + f2);
        }
    }
}
