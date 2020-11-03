//////////////////////////////////////////////////
/// The Main mediator type for entities interacting, components can subscribe actions to individual events to effect what it does
/// Last Updated: Version 0.0.0 11/01/2020
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

        Dictionary<string, object> parameters;
        Dictionary<string, Event> child_events;
        //TODO: Make the elements of this a data structure to make code easier to debug/read
        SimplePriorityQueue<Action<Event>> updates;

        public Event()
        {
            id = "NONE";
            type = "NONE";
            priority = (1 << 30);
            parameters = new Dictionary<string, object>();
            tags = new List<string>();
            child_events = new Dictionary<string, Event>();
        }

        public Event(string id, string type, int priority, Action<Entity> to_call)
        {
            this.id = id;
            this.type = type;
            this.priority = priority;
            parameters = new Dictionary<string, object>();
            updates = new SimplePriorityQueue<Action<Event>>();
            tags = new List<string> { type };
            child_events = new Dictionary<string, Event>();
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
            child_events = new Dictionary<string, Event>();
        }

        public static Event NewEvent(string event_type)
        {
            return new Event(EventManager.instance.GetEvent(event_type));
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

        public void Invoke(Entity e, List<(Entity,string)> targets = null)
        {
            AddUpdates(e);
            while(updates.Count > 0)
            {
                updates.Dequeue()?.Invoke(this);
            }
            if (targets != null)
            {
                foreach((Entity,string) target in targets)
                {
                    if (child_events.ContainsKey(target.Item2))
                    {
                        new Event(child_events[target.Item2]).Invoke(target.Item1);
                        continue;
                    }
                    Debug.LogWarning($"Could not find event: {target.Item2}");
                }
            }
        }

        public void AddUpdates(Entity e, string child_id = "", string child_type = "")
        {
            if(child_id == "")
            {
                foreach (IBaseComponent b in e.components.Values)
                {
                    if (b.Trigger(this) == false)
                    {
                        return;
                    }
                }
            } else
            {
                if (child_events.ContainsKey(child_id) == false)
                {
                    child_events.Add(child_id, NewEvent(child_type != "" ? child_type : id));
                }
                child_events[child_id].AddUpdates(e);
            }
        }

        public void Cancel(string child_id = "")
        {
            if(child_id == "")
            {
                while (updates.Count > 0)
                {
                    updates.Dequeue();
                }
            } else
            {
                child_events[child_id].Cancel();
            }
        }

        public void AddUpdate(Action<Event> a_e, int priority = 100, string child_id = "", string child_type = "")
        {
            if(child_id == "")
            {
                updates.Enqueue(a_e, priority);
            }
            else
            {
                if (child_events.ContainsKey(child_id) == false)
                {
                    child_events.Add(child_id, NewEvent(child_type != "" ? child_type : id));
                }
                child_events[child_id].AddUpdate(a_e, priority);
            }
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

        public void SetParamValue<T>(string key, T o, Func<T, T, T> combine_func = null, string event_id = "", string event_type = "")
        {
            if (event_id == "")
            {
                if (parameters.ContainsKey(key))
                {
                    if (combine_func == null)
                    {
                        Debug.LogWarning("An event component was called to change without a combiner function");
                        parameters[key] = o;
                    }
                    else
                    {
                        parameters[key] = combine_func.Invoke((T)parameters[key], o);
                    }
                }
                else
                {
                    parameters.Add(key, o);
                }
            }
            else
            {
                if (child_events.ContainsKey(event_id) == false)
                {
                    child_events.Add(event_id, NewEvent(event_type != "" ? event_type : id));
                }
                child_events[event_id].SetParamValue(key, o, combine_func);
            }
        }

        public Event GetChildEvent(string child_id)
        {
            return child_events[child_id];
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
            s += $"\nchild events:";
            foreach(KeyValuePair<string,Event> e in child_events)
            {
                s += $"\n{e.Key}: {e.Value.ToString()}";
            }
            return s;
        }
    }
}

/*
 * Idea on when an event is going to create a new event:
 *      Need function to make this not dumb to code
 *      SetParamValue(event_id, param_id, param_value, combine_func)? 
 *      Invoke(List<(entity_target, event_id)>)?
 */