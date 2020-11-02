using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class ActiveAttack : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }
        public string attack_id;
        public Entity attack;

        public ActiveAttack()
        {
        }

        public void SetAttack()
        {
            attack = Entity.Manager()[attack_id];
        }

        public void SetAttack(string attack_id)
        {
            this.attack_id = attack_id;
            attack = Entity.Manager()[attack_id];
        }

        public bool Trigger(Event e)
        {
            bool eval = true;
            if(attack == null)
            {
            } else
            {
                if (e.type == "DoDamage")
                {
                    e.AddUpdates(attack);
                    e.SetParamValue("take_damage_event", EventManager.instance.GetEvent("TakeDamage"), (e1, e2) => { return e1; });
                    e.AddUpdate((e1) => { EventManager.instance.DoEvent(e1.GetParamValue<Entity>("target_entity"),e1.GetParamValue<Event>("take_damage_event")); }, 1000);
                }
            }
            return eval;
        }
    }
}
