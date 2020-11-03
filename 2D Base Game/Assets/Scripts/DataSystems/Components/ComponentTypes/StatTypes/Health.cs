using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Health : IBaseComponent, IInspectorDisplay
    {
        public SubscriberEvent subscriber { get; set; }
        public InspectorData inspector_data { get => MainGame.instance.display_data["Health"]; set { } }
        
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

        public Health(JObject p)
        {
            base_value = curr_value = 1;

            if(p["base_value"] != null)
            {
               base_value = curr_value = p.Value<int>("base_value");
            }
            if(p["curr_value"] != null)
            {
                curr_value = p.Value<int>("curr_value");
            }

        }

        public bool Trigger(Event e)
        {
            bool eval = true;
            switch (e.type)
            {
                case "TakeDamage":
                    e.AddUpdate((t) => {
                        DiceGroup dg = new DiceGroup();
                        foreach(string s in e.GetParamValue<string>("damage_type").Split(','))
                        {
                            if(s != "")
                            {
                                dg += e.GetParamValue<DiceGroup>(s + "_damage");
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

        public string DisplayText()
        {
            return $"Health: {curr_value} / {base_value}";
        }
    }
}
