using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace HighKings
{
    public class Activation : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }
        public List<string> events;

        public Activation()
        {
            events = new List<string>();
        }

        public Activation(JObject jobj)
        {
            events = new List<string>();
            if(jobj["events"] != null)
            {
                foreach(JToken tok in jobj["events"].Values())
                {
                    events.Add(tok.Value<string>());
                }
            }
        }

        public Activation(Activation a)
        {
            a.events = new List<string>(a.events);
        }

        public bool Trigger(Event e)
        {
            
            throw new System.NotImplementedException();
        }

        void PushEvents(Event e)
        {
            EventQueue es = new EventQueue();
            foreach(string s in events)
            {
                es.RegisterEvent(EventManager.instance.GetEvent(s));
            }
        }
    }

}
