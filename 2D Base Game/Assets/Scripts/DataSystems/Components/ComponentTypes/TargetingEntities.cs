using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TargetingEntities : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }

        List<Entity> targeting_entities;

        public TargetingEntities()
        {
        }

        public TargetingEntities(TargetingEntities t)
        {
            targeting_entities = new List<Entity>(t.targeting_entities);
        }

        public TargetingEntities(Entity e)
        {
            targeting_entities = new List<Entity> { e };
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="j"></param>
        public TargetingEntities(JProperty j)
        {
            if (j.Value["entity_id"] != null)
            {
                EntityManager.instance.GetEntityFromId(j.Value.Value<string>("entity_id"));
            }
        }

        public List<Entity> GetTargetingEntities()
        {
            return targeting_entities;
        }

        public void SetTargetedEntity(Entity e, bool call_subscribers = false)
        {
            if(call_subscribers)
                subscriber.OperateBeforeOnComp();
            if (targeting_entities.Contains(e))
            {
            } else
            {
                targeting_entities.Add(e);
            }
            if (call_subscribers)
                subscriber.OperateAfterOnComp();
        }

        public void SetTargetedEntities(List<Entity> es, bool call_subscribers = false)
        {
            if(call_subscribers)
                subscriber.OperateBeforeOnComp();
            foreach(Entity e in es)
            {
                SetTargetedEntity(e);
            }
            if (call_subscribers)
                subscriber.OperateAfterOnComp();
        }

        public void RemoveTargetedEntity(Entity e, bool call_subscribers = false)
        {
            if (call_subscribers)
                subscriber.OperateBeforeOnComp();
            if (targeting_entities.Contains(e))
            {
                targeting_entities.Remove(e);
            }
            if (call_subscribers)
                subscriber.OperateAfterOnComp();
        }

        public void RemoveTargetedEntities(List<Entity> es, bool call_subscribers = false)
        {
            if (call_subscribers)
                subscriber.OperateBeforeOnComp();
            foreach(Entity e in es)
            {
                RemoveTargetedEntity(e);
            }
            if (call_subscribers)
                subscriber.OperateAfterOnComp();
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