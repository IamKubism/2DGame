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
                eval &= false;
            } else
            {
                if (e.type == "DoDamage")
                {
                    e.AddUpdates(attack);
                }
            }
            return eval;
        }
    }
}
