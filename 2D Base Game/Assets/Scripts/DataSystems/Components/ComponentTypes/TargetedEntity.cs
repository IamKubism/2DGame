using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TargetedEntity : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }

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

        public void SetTargetedEntity(Entity e, bool call_listeners = false)
        {
            if (call_listeners)
                subscriber.OperateBeforeOnComp();
            targeted_entity = e;
            if (call_listeners)
                subscriber.OperateAfterOnComp();
        }


        public bool Trigger(Event e)
        {
            e.AddUpdate(SetTargetEntityParam, 0);
            switch (e.type)
            {
                case "DoDamage":
                    e.AddUpdate((v) =>
                    {
                        EventManager.instance.AddEvent(targeted_entity, new Event(v, "TakeDamage"));
                    }, 100);
                    break;
                default:
                    break;
            }
            e.AddUpdate(SetTargetedEntity, 100);
            return true;
        }

        public static implicit operator Entity(TargetedEntity e)
        {
            return e.targeted_entity;
        }

        void SetTargetEntityParam(Event e)
        {
            if(targeted_entity != null)
                e.SetParamValue("target_entity", GetTargetedEntity(), (e1, e2) => { return e2; });
        }

        void SetTargetedEntity(Event e)
        {
            SetTargetedEntity(e.GetParamValue<Entity>("target_entity"), true);
        }

    }
}

