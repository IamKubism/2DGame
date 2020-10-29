using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Priority_Queue;
using System.Linq;

namespace HighKings
{
    public class Event
    {
        public string id;
        public string type;
        public int priority;
        public List<string> tags;
        public Dictionary<string, object> parameters;
        SimplePriorityQueue<Action<Event>> updates;
        Action<Entity> to_call;

        public Event()
        {
            id = "NONE";
            type = "NONE";
            priority = 100;
            parameters = new Dictionary<string, object>();
            tags = new List<string>();
        }

        public Event(string id, string type, int priority, Action<Entity> to_call)
        {
            this.id = id;
            this.type = type;
            this.priority = priority;
            this.to_call = to_call;
            parameters = new Dictionary<string, object>();
            updates = new SimplePriorityQueue<Action<Event>>();
            tags = new List<string> { type };
        }

        public Event(JProperty p)
        {
            parameters = new Dictionary<string, object>();
            id = p.Name;
            type = p.Value.Value<string>("type");
            priority = p.Value.Value<int>("priority");
            if(p.Value["tags"] != null)
            {
                tags = new List<string> { type };
                foreach(JToken tok in p.Value["tags"].ToList())
                {
                    tags.Add(tok.Value<string>());
                }
            }
        }

        public Event(Event el)
        {
            parameters = new Dictionary<string, object>(el.parameters);
            id = el.id;
            type = el.type;
            priority = el.priority;
            to_call = el.to_call;
            tags = new List<string>(el.tags);
        }

        public Event(Event el, string forward_type)
        {
            parameters = new Dictionary<string, object>(el.parameters);
            id = forward_type;
            type = forward_type;
            tags = new List<string>(el.tags);
        }

        public void Invoke(Entity e)
        {
            foreach(IBaseComponent b in e.components.Values)
            {
                b.Trigger(this);
            }
            while(updates.Count > 0)
            {
                updates.Dequeue()?.Invoke(this);
            }
            to_call?.Invoke(e);
        }

        public void AddUpdates(Entity e)
        {
            foreach (IBaseComponent b in e.components.Values)
            {
                b.Trigger(this);
            }
        }

        public void Cancel()
        {
            while(updates.Count > 0)
            {
                updates.Dequeue();
            }
        }

        public void AddUpdate(Action<Event> a_e, int priority = 100)
        {
            updates.Enqueue(a_e, priority);
        }

        public object GetParamValue(string key)
        {
            if(!parameters.TryGetValue(key, out object to_return))
            {
                Debug.LogWarning($"Could not find Key {key} for parameters with event {id}");
            }
            return to_return;
        }

        public bool HasParamValue(string key)
        {
            return parameters.ContainsKey(key);
        }

        public void SetParamValue(string key, object o, Func<object, object, object> combine_func = null)
        {
            if (parameters.ContainsKey(key))
            {
                if(combine_func == null)
                {
                    Debug.LogWarning("An event component was called to change without a combiner function");
                    parameters[key] = o;
                } else
                {
                    parameters[key] = combine_func.Invoke(parameters[key], o);
                }
            }
            else
            {
                parameters.Add(key, o);
            }
        }
    }
}

