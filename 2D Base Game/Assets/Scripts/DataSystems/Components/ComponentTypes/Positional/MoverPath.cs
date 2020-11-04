using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HighKings
{
    public class MoverPath : IBaseComponent
    {
        public Path_Astar path;
        public Entity curr_tile;
        public Entity next_tile;
        public SubscriberEvent subscriber { get; set; }
        public FloatMinMax progress;

        public MoverPath()
        {
            progress = new FloatMinMax();
        }

        public MoverPath(JObject obj)
        {
            progress = new FloatMinMax();
            if(obj["progress"] != null)
            {
                progress = new FloatMinMax(obj["progress"].Value<float>("curr"), obj["progress"].Value<float>("max"), obj["progress"].Value<float>("min"), obj["progress"].Value<float>("dt"));
            }
            if(obj["curr_tile"] != null)
            {
                curr_tile = obj.Value<string>("curr_tile");
            }
        }

        public bool Trigger(Event e)
        {
            bool eval = true;
            if (e.tags.Contains("SetMoveTarget"))
            {
                e.AddUpdate(SetPath, 100);
            }
            if (e.tags.Contains("GameTime") && (curr_tile != next_tile))
            {
                e.AddUpdate(ProgressMovement, 101);
                e.AddUpdate(MoveToNextTile, 98);
            }
            return eval;
        }

        void ProgressMovement(Event e)
        {
            float dt = 0f;
            //progress += (float)e.GetParamValue("move_speed"); //TODO
            if (e.HasParamValue("move_progress"))
            {
                dt = e.GetParamValue<float>("time");
            }
            progress += dt;
            float norm_prog = progress.NormalizedByMax();
            Vector3 vec = (norm_prog)*next_tile.GetComponent<Position>().t_r - (1-norm_prog)*curr_tile.GetComponent<Position>().t_r;
            e.SetParamValue("displaced_position", vec, (v1, v2) => { return v2; });
            e.SetParamValue("continue_update", true, (b1, b2) => { return b2; });
        }


        void SetPath(Event e)
        {
            if(!e.HasParamValue("curr_tile") || !e.HasParamValue("dest_tiles"))
            {
                Debug.LogWarning("Tried to set path without enough information");
                return;
            }
            path = new Path_Astar(MainGame.instance.world.graph, e.GetParamValue<Entity>("curr_tile"), e.GetParamValue<List<Entity>>("dest_tiles"), MovementCalculator.test_calculator);
            progress = new FloatMinMax(); //TODO
            next_tile = path.DeQueue();
        }

        void MoveToNextTile(Event e)
        {
            while (progress.IsOverMax())
            {
                curr_tile = next_tile;
                if (path.Length() > 0)
                {
                    Entity next = path.DeQueue();
                    next_tile = next;
                    Entity parent = e.GetParamValue<Entity>("parent_entity");
                    Event cost = Entity.eventManager().DoEvent(parent, "TileCost");
                    float f = cost.GetParamValue<float>("tile_cost");
                    if(f > 0)
                    {
                        progress += -f;
                        e.SetParamValue("curr_tile", curr_tile, (p, n) => { return n; });
                    } else
                    {
                        //TODO: Get it to try to reset the thing
                        progress.Reset();
                        path = null;
                        e.SetParamValue("continue_update", false, (b1, b2) => { return b2; });
                        //e.SetParamValue("goal_failed", false, (b1, b2) => { return b2; });
                    }
                }
                if (path.Length() == 0)
                {
                    progress.Reset();
                    path = null;
                    e.SetParamValue("continue_update", false, (b1, b2) => { return b2; });
                }
            }
        }

        void SetCurrTile(Event e)
        {
            e.GetParamValue<Entity>("curr_tile");
        }

    }

}
