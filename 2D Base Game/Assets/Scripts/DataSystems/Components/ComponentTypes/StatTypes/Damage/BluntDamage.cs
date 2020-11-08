﻿using System.Collections;
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
                e.AddUpdate(AddToTotalDamage, 100, "BaseDamage", "TakeDamage");
            }
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

