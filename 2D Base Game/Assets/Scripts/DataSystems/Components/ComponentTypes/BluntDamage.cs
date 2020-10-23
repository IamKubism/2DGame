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
        SubscriberEvent<BluntDamage> listener;

        [JsonProperty]
        int base_damage;

        public BluntDamage()
        {
            base_damage = 0;
        }

        public BluntDamage(BluntDamage prot)
        {
            base_damage = prot.base_damage;
        }

        public BluntDamage(JProperty p)
        {
            base_damage = p.Value<int>("base_damage");
        }

        public void SetListener<T>(SubscriberEvent<T> subscriber) where T : IBaseComponent
        {
            listener = (SubscriberEvent<BluntDamage>)Convert.ChangeType(subscriber, typeof(SubscriberEvent<BluntDamage>));
        }
    }
}

