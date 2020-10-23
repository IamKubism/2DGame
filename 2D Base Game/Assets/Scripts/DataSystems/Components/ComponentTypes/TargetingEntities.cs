using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TargetingEntities : IBaseComponent
    {
        SubscriberEvent<TargetingEntities> listener;
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

        public void SetTargetedEntity(Entity e, bool call_listeners = false)
        {
            if(call_listeners)
                listener.OperateBeforeOnComp();
            if (targeting_entities.Contains(e))
            {
            } else
            {
                targeting_entities.Add(e);
            }
            if (call_listeners)
                listener.OperateAfterOnComp();
        }

        public void SetTargetedEntities(List<Entity> es, bool call_listeners = false)
        {
            if(call_listeners)
                listener.OperateBeforeOnComp();
            foreach(Entity e in es)
            {
                SetTargetedEntity(e);
            }
            if (call_listeners)
                listener.OperateAfterOnComp();
        }

        public void RemoveTargetedEntity(Entity e, bool call_listeners = false)
        {
            if (call_listeners)
                listener.OperateBeforeOnComp();
            if (targeting_entities.Contains(e))
            {
                targeting_entities.Remove(e);
            }
            if (call_listeners)
                listener.OperateAfterOnComp();
        }

        public void RemoveTargetedEntities(List<Entity> es, bool call_listeners = false)
        {
            if (call_listeners)
                listener.OperateBeforeOnComp();
            foreach(Entity e in es)
            {
                RemoveTargetedEntity(e);
            }
            if (call_listeners)
                listener.OperateAfterOnComp();
        }

        public void SetListener<T>(SubscriberEvent<T> subscriber) where T : IBaseComponent
        {
            listener = (SubscriberEvent<TargetingEntities>)System.Convert.ChangeType(subscriber, typeof(SubscriberEvent<TargetingEntities>));
        }
    }
}