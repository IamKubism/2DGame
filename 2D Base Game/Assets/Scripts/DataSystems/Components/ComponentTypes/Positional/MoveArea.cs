using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace HighKings
{
    [JsonObject]
    public class MoveArea : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }

        public List<Entity> tile_entities;

        public MoveArea()
        {
            tile_entities = new List<Entity>();
        }

        public MoveArea(Entity e)
        {
            tile_entities = new List<Entity> { e };
        }

        public MoveArea(MoveArea t)
        {
            tile_entities = t;
        }

        public MoveArea(JProperty j)
        {
            tile_entities = new List<Entity>();
        }

        public void SetMoveTile(Entity e, bool call_listeners = false)
        {
            if (call_listeners)
                subscriber.OperateBeforeOnComp();
            tile_entities = new List<Entity>{e};
            if (call_listeners)
                subscriber.OperateAfterOnComp();
        }

        public void SetMoveTiles(List<Entity> es, bool call_listeners = false)
        {
            if (call_listeners)
                subscriber.OperateBeforeOnComp();
            tile_entities = new List<Entity>(es);
            if (call_listeners)
                subscriber.OperateAfterOnComp();
        }

        public bool Trigger(Event e)
        {
            bool eval = true;
            if (e.tags.Contains("SetMoveTarget"))
            {
                //Some way to get the target it needs to get set to
            }
            return eval;
        }

        public static implicit operator MoveArea(Entity e)
        {
            return new MoveArea(e);
        }

        public static implicit operator MoveArea(List<Entity> e)
        {
            return new MoveArea(e);
        }

        public static implicit operator List<Entity>(MoveArea m)
        {
            return new List<Entity>(m.tile_entities);
        }
    }
}

