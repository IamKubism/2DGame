using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Health : IBaseComponent
    {
        SubscriberEvent<Health> listener;

        [JsonProperty]
        public int curr_value;

        [JsonProperty]
        public int base_value;

        public Health()
        {
            curr_value = 1;
            base_value = 1;
        }

        public Health(int base_value)
        {
            curr_value = base_value;
            this.base_value = base_value;
        }

        public Health(int curr_value, int base_value)
        {
            this.curr_value = curr_value;
            this.base_value = base_value;
        }

        public Health(Health h)
        {
            curr_value = h.curr_value;
            base_value = h.curr_value;
        }

        public Health(JProperty p)
        {
            if(p.Value["base_value"] != null)
            {
               base_value = p.Value.Value<int>("base_value");
            }
            if(p.Value["curr_value"] != null)
            {
                curr_value = p.Value.Value<int>("curr_value");
            } else
            {
                curr_value = base_value;
            }
        }

        public void SetListener<T>(SubscriberEvent<T> subscriber) where T : IBaseComponent
        {
            listener = (SubscriberEvent<Health>)System.Convert.ChangeType(subscriber, typeof(SubscriberEvent<Health>));
        }

        public bool Trigger(Event e)
        {
            bool eval = true;
            switch (e.type)
            {
                case "TakeDamage":
                    e.AddUpdate((t) => {
                        DiceGroup dg = new DiceGroup();
                        foreach(string s in ((string)e.GetParamValue("damage_type")).Split(','))
                        {
                            if(s != "")
                            {
                                dg += (DiceGroup)e.GetParamValue(s + "_damage");
                            }
                        }
                        curr_value -= (int)dg;
                    }, 100);
                    break;
                default:
                    eval = false;
                    break;
            }
            return eval;
        }
    }
}
