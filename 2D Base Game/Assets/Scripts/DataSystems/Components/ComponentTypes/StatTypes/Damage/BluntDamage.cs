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
            d = new Dice(prot.d);
        }

        public BluntDamage(JObject p)
        {
            d = new Dice();
            if(p["d"] != null)
            {
                d = new Dice(p.Value<string>("d"));
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
            if (e.tags.Contains("DoDamage"))
            {
                e.AddUpdate(AddDamageType, 10, "BaseDamage", "TakeDamage");
                e.AddUpdate(AddToTotalDamage, 99, "BaseDamage", "TakeDamage");
            }
            //switch (e.type)
            //{
            //    case "DoDamage":
            //        e.AddUpdate(
            //            (v) => {
            //                v.SetParamValue("blunt_damage", new DiceGroup(d));
            //                if(!v.HasParamValue("total_damage"))
            //                    v.SetParamValue("total_damage", new DiceGroup());
            //                    v.SetParamValue("total_damage", new DiceGroup());
            //                v.SetParamValue("damage_type", v.GetParamValue<string>("damage_type") + ",blunt");
            //            }, 1);
            //        e.GetParamValue<Event>("take_damage_event").AddUpdate((ev) => { ev.SetParamValue("blunt_damage", e.GetParamValue<DiceGroup>("blunt_damage")); },1);
            //        e.GetParamValue<Event>("take_damage_event").AddUpdate((ev) => { ev.SetParamValue("total_damage", ev.GetParamValue<float>("blunt_damage"), (d1, d2) => { return d1 + d2; }); },50);
            //        break;
            //}
            return eval;
        }

        void AddDamageType(Event e)
        {
            e.SetParamValue("blunt_damage", new DiceGroup(d), (d1, d2) => { return d1 + d2; }, "TotalDamage", "TakeDamage");
        }

        void AddToTotalDamage(Event e)
        {
            e.SetParamValue<int>("total_damage", e.GetParamValue<DiceGroup>("blunt_damage"), (i1, i2) => { return i1 + i2; });
        }
    }
}

