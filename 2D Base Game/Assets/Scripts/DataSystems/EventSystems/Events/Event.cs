//////////////////////////////////////////////////
/// The Main mediator type for entities interacting, components can subscribe actions to individual events to effect what it does
/// Last Updated: Version 0.0.0 10/30/2020
/// Updater: _Kubism
//////////////////////////////////////////////////

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
        //TODO: Make the elements of this a data structure to make code easier to debug/read
        SimplePriorityQueue<Action<Event>> updates;

        public Event()
        {
            id = "NONE";
            type = "NONE";
            priority = (1 << 30);
            parameters = new Dictionary<string, object>();
            tags = new List<string>();
        }

        public Event(string id, string type, int priority, Action<Entity> to_call)
        {
            this.id = id;
            this.type = type;
            this.priority = priority;
            parameters = new Dictionary<string, object>();
            updates = new SimplePriorityQueue<Action<Event>>();
            tags = new List<string> { type };
        }

        public Event(JProperty p)
        {
            parameters = new Dictionary<string, object>();
            id = p.Name;
            type = p.Value["type"] != null ? p.Value.Value<string>("type") : "NULL";
            priority = p.Value["priority"] != null ? p.Value.Value<int>("priority") : (1 << 30);
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
            tags = new List<string>(el.tags);
        }

        public Event(Event el, string forward_type)
        {
            parameters = new Dictionary<string, object>(el.parameters);
            Event forward = EventManager.instance.GetEvent(forward_type);
            id = forward_type;
            type = forward.type;
            tags = new List<string>(forward.tags);
        }

        public void Invoke(Entity e)
        {
            AddUpdates(e);
            while(updates.Count > 0)
            {
                updates.Dequeue()?.Invoke(this);
            }
        }

        public void AddUpdates(Entity e)
        {
            foreach (IBaseComponent b in e.components.Values)
            {
                if(b.Trigger(this) == false)
                {
                    return;
                }
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

        public T GetParamValue<T>(string key)
        {
            if(!parameters.TryGetValue(key, out object to_return))
            {
                Debug.LogWarning($"Could not find Key {key} for parameters with event {id}");
            }
            return (T)to_return;
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

        public void SetParamValue<T>(string key, T o, Func<T, T, T> combine_func = null)
        {
            if (parameters.ContainsKey(key))
            {
                if(combine_func == null)
                {
                    Debug.LogWarning("An event component was called to change without a combiner function");
                    parameters[key] = o;
                } else
                {
                    parameters[key] = combine_func.Invoke((T)parameters[key], o);
                }
            }
            else
            {
                parameters.Add(key, o);
            }
        }

        public override string ToString()
        {
            string s = $"id: {id},\ntags: [";
            for(int i = 0; i < tags.Count - 1; i+= 1)
            {
                s += $" {tags[i]},";
            }
            s += $" {tags.Last()}]\nParams: [";
            int j = 0;
            KeyValuePair<string, object> last;
            foreach(KeyValuePair<string, object> ko in parameters)
            {
                j += 1;
                if(j > parameters.Count)
                {
                    last = ko;
                    break;
                }
                s += $" {{ {ko.Key} : {ko.Value.ToString()} }},";
            }
            s += $" {{ {last.Key} : {last.Value.ToString()} }}]";
            return s;
        }
    }
}

