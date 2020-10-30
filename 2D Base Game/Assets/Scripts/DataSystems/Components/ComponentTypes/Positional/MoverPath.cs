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
        public SubscriberEvent subscriber { get; set; }
        public FloatMinMax progress;

        public MoverPath()
        {
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
            if (e.tags.Contains("ProgressMovement"))
            {
                e.AddUpdate(ProgressMovement, 98);
                e.AddUpdate(MoveToNextTile, 99);
            }
            return eval;
        }

        void ProgressMovement(Event e)
        {
            //progress += (float)e.GetParamValue("move_speed"); //TODO
            e.SetParamValue("move_progress", 0.0001f, (f1, f2) => { return f1; });
            progress += e.GetParamValue<float>("move_progress");
        }

        void SetPath(Event e)
        {
            if(!e.HasParamValue("curr_tile") || !e.HasParamValue("dest_tiles"))
            {
                Debug.LogWarning("Tried to set path without enough information");
                return;
            }
            path = new Path_Astar(MainGame.instance.world.graph, (Entity)e.GetParamValue("curr_tile"), (List<Entity>)e.GetParamValue("dest_tiles"), MovementCalculator.test_calculator);
            progress = new FloatMinMax(); //TODO
        }

        void MoveToNextTile(Event e)
        {
            while (progress.IsOverMax())
            {
                if (path.Length() > 0)
                {
                    Entity next = path.DeQueue();
                    Entity parent = (Entity)e.GetParamValue("invoker");
                    Event cost = Entity.Event_Manager().PassEvent(parent, "TileCost");
                    float f = cost.GetParamValue<float>("tile_cost");
                    if(f > 0)
                    {
                        curr_tile = next;
                        progress += (-f);
                        e.SetParamValue("curr_tile", curr_tile, (p, n) => { return n; });
                    } else
                    {
                        //TODO: Get it to try to reset the thing
                        progress.Reset();
                        path = null;
                        e.Cancel();
                    }
                }
                if (path.Length() == 0)
                {
                    progress.Reset();
                    path = null;
                }
            }
        }
    }

}
