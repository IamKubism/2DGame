using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BluntDamage : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }

        Dice d;
        public BluntDamage()
        {
            d = new Dice();
        }

        public BluntDamage(BluntDamage prot)
        {
            prot.d = new Dice(d);
        }

        public BluntDamage(JProperty p)
        {
            d = new Dice();
            if(p.Value["dice"] != null)
            {
                d = new Dice(p.Value.Value<string>("dice"));
            }
        }

        public bool SetSubscriberListener(Action<IBaseComponent> action, bool before_after)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO: This kind of thing could be for sure defined in some text document
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool Trigger(Event e)
        {
            bool eval = true;
            switch (e.type)
            {
                case "DoDamage":
                    e.AddUpdate(
                        (v) => {
                            v.SetParamValue("blunt_damage", new DiceGroup(d));
                            if(!v.HasParamValue("total_damage"))
                                v.SetParamValue("total_damage", new DiceGroup());
                            v.SetParamValue("damage_type", v.GetParamValue("damage_type") + ",blunt");
                        }, 1);
                    break;
            }
            return eval;
        }
    }
}

