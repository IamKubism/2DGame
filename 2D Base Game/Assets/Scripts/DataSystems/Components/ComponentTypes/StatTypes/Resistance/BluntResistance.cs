using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BluntResistance : IBaseComponent
    {
        SubscriberEvent<BluntResistance> listener;

        [JsonProperty]
        public int res_val;

        public void SetListener<T>(SubscriberEvent<T> subscriber) where T : IBaseComponent
        {
            listener = (SubscriberEvent<BluntResistance>)System.Convert.ChangeType(subscriber, typeof(SubscriberEvent<BluntResistance>));
        }

        public bool Trigger(Event e)
        {
            bool eval = true;
            switch (e.type)
            {
                case "TakeDamage":
                    e.AddUpdate((v) =>
                    {
                        v.SetParamValue("blunt_damage", ((DiceGroup)v.GetParamValue("blunt_value"))+(-res_val));
                    }, 10);
                    break;
                default:
                    break;
            }
            return eval;
        }
    }

}
