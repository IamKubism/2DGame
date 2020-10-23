using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TargetedEntity : IBaseComponent
    {
        SubscriberEvent<TargetedEntity> listener;
        Entity targeted_entity;

        public TargetedEntity()
        {
        }

        public TargetedEntity(TargetedEntity t)
        {
            targeted_entity = t.targeted_entity;
        }

        public TargetedEntity(Entity e)
        {
            targeted_entity = e;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="j"></param>
        public TargetedEntity(JProperty j)
        {
            if(j.Value["entity_id"] != null)
            {
                EntityManager.instance.GetEntityFromId(j.Value.Value<string>("entity_id"));
            }
        }

        public Entity GetTargetedEntity()
        {
            return targeted_entity;
        }

        public void SetTargetedEntity(Entity e)
        {

        }

        public void SetListener<T>(SubscriberEvent<T> subscriber) where T : IBaseComponent
        {
            listener = (SubscriberEvent<TargetedEntity>)System.Convert.ChangeType(subscriber, typeof(SubscriberEvent<TargetedEntity>));
        }
    }
}

