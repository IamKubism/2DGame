using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace Psingine
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
            if(e.tags.Contains(MovementEvents.SetMovementData.ToString()))
            {
                e.AddUpdate(SetTerrainCost, 0);
            }
            if (e.tags.Contains(MovementEvents.ComputeMovementProgress.ToString()))
            {
                e.AddUpdate(SetMovementProgress, 40);
            }
            return eval;
        }

        public void SetTerrainCost(Event e)
        {
            e.SetParamValue(MovementParams.terrain_cost, move_cost, (o1,o2) => { return AddCosts(o1, o2); });
        }

        public void SetMovementProgress(Event e)
        {
            e.SetParamValue(MovementParams.terrain_cost, move_cost > 0 ? 1 / move_cost : -1f, (o1, o2) => { return MultCosts(o1, o2); });
        }

        public float AddCosts(float f1, float f2)
        {
            return Mathf.Min(Mathf.Sign(f1),Mathf.Sign(f2))*Mathf.Abs(f1 + f2);
        }

        public float MultCosts(float f1, float f2)
        {
            return f1 > 0 && f2 > 0 ? Mathf.Min(Mathf.Sign(f1),Mathf.Sign(f2))*Mathf.Abs(f1*f2) : -1f;
        }

        public void CopyData(IBaseComponent comp)
        {
            throw new NotImplementedException();
        }
    }

    public class TileMoveCost : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }

        public float cost;

        public TileMoveCost()
        {
            cost = 1f;
        }

        public void CopyData(IBaseComponent comp)
        {
            throw new NotImplementedException();
        }

        public bool Trigger(Event e)
        {
            throw new NotImplementedException();
        }
    }

    public class ComputeTileTerrainCost : IEventObserver
    {
        public static ComputeTileTerrainCost instance;

        public ComputeTileTerrainCost()
        {
            if(instance == null)
            {
                instance = this;
            }

            EventManager.instance.AddObserver(new string[] { BaseEvents.compute_tile_cost.ToString() }, instance);
        }

        public bool Observe(Event e)
        {
            return true;
        }

        public void Compute(Event e)
        {
            if(e.TryGetTempEntity(EventParams.target_entity.ToString(), out Entity target_tile))
            {
                e.TryGetTempEntity(EventParams.tile_cost_entity.ToString(), out Entity tile_cost, true);
                TileMoveCost t_m_c = (TileMoveCost)tile_cost.TryAddComponent(nameof(TileMoveCost), new TileMoveCost());
                if (e.TryGetRelevantEntity(EventParams.moving_entity.ToString(), out Entity mover))
                {
                    if(mover.TryGetComponent<Walker>(out Walker w))
                    {
                    }
                }
            }
        }
    }
}
