using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BluntResistance : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }


        [JsonProperty]
        public int res_val;

        public bool Trigger(Event e)
        {
            bool eval = true;
            switch (e.type)
            {
                case "TakeDamage":
                    e.AddUpdate((v) =>
                    {
                        v.SetParamValue("blunt_damage", ((DiceGroup)v.GetParamValue("blunt_damage"))+(-res_val));
                    }, 10);
                    break;
                default:
                    break;
            }
            return eval;
        }

        public bool SetSubscriberListener(Action<IBaseComponent> action, bool before_after)
        {
            throw new NotImplementedException();
        }
    }

}
