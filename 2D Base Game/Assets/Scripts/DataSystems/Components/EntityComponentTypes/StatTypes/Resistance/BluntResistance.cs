using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Psingine
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BluntResistance : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }

        [JsonProperty]
        public int res_val;

        public BluntResistance()
        {
            res_val = 0;
        }

        public BluntResistance(JObject obj)
        {
            if(obj["res_val"] != null)
            {
                obj.Value<int>("res_val");
            }
        }

        public BluntResistance(BluntResistance prot)
        {
            res_val = prot.res_val;
        }

        public bool Trigger(Event e)
        {
            bool eval = true;
            if(e.type == "TakeDamage")
            {
                e.AddUpdate(ReduceDamage, 10);
            }
            return eval;
        }

        public void ReduceDamage(Event e)
        {
            e.SetParamValue(DamageParams.blunt_damage, new DiceGroup(-res_val), (i1, i2) => { return i1 + i2; });
        }

        public void CopyData(IBaseComponent comp)
        {
            throw new NotImplementedException();
        }
    }

}
