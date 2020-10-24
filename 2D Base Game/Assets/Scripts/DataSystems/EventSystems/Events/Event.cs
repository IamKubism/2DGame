using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public class Event
    {
        public string id;
        public string type;
        public int priority;
        Dictionary<string, object> parameters;
        Action<Entity> to_call;

        public Event()
        {
            id = "NONE";
            type = "NONE";
            priority = 100;
            parameters = new Dictionary<string, object>();
        }

        public Event(string id, string type, int priority, Action<Entity> to_call)
        {
            this.id = id;
            this.type = type;
            this.priority = priority;
            this.to_call = to_call;
            parameters = new Dictionary<string, object>();
        }

        public Event(JProperty p)
        {
            parameters = new Dictionary<string, object>();
            id = p.Name;
            type = p.Value.Value<string>("type");
            priority = p.Value.Value<int>("priority");

        }

        public Event(Event el)
        {
            parameters = new Dictionary<string, object>(el.parameters);
            id = el.id;
            type = el.type;
            priority = el.priority;
            to_call = el.to_call;
        }

        public void Invoke(Entity e)
        {
            to_call?.Invoke(e);
        }

        public object GetParamValue(string key)
        {
            parameters.TryGetValue(key, out object to_return);
            return to_return;
        }
        
    }
}

