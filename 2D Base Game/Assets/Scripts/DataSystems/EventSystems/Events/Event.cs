//////////////////////////////////////////////////
/// The Main mediator type for entities interacting, components (or really anything) can subscribe actions to individual events to effect what it does
/// Essentially its dynamically generated functions
/// Last Updated: Version 0.0.0 12/21/2020
/// Updater: _Kubism
//////////////////////////////////////////////////

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using Priority_Queue;
using System.Linq;


namespace Psingine
{
    public enum BaseEvents
    {
        pass_time,
        take_damage,
        make_damage,
        compute_tile_cost
    }

    public enum EventParams
    {
        invoking_entity,
        targets,
        time_dt,
        job_progress,
        damage_entity,
        total_damage,
        target_entity,
        attack_entity,
        attacker_entity,
        tile_cost_entity,
        moving_entity
    }

    public enum PriorityRange
    {
        first = 0,
        small = (1 << 4),
        medium = (1 << 6),
        large = (1 << 8)
    }

    public class Event
    {
        public string id;
        public string type;
        public int priority;
        public HashSet<string> tags;

        //TODO: Make the elements of this a data structure to make code easier to debug/read
        Dictionary<string, object> parameters;

        Dictionary<string, Entity> temp_entities;
        Dictionary<string, Entity> relevant_entities;
        List<Event> triggered_events;

        //TODO: Make the elements of this a data structure to make code easier to debug/read
        List<(Action<Event>, int)> priorities;

        List<EventAlterer> alters_to_call;
        Dictionary<string, List<EventAlterer>> alters_by_tag;

        SimplePriorityQueue<EventAlterer> alter_queue;

        public Event()
        {
            id = "NONE";
            type = "NONE";
            priority = (1 << 30);
            parameters = new Dictionary<string, object>();
            tags = new HashSet<string>();
            priorities = new List<(Action<Event>, int)>();
            temp_entities = new Dictionary<string, Entity>();
            relevant_entities = new Dictionary<string, Entity>();
            triggered_events = new List<Event>();
            alters_to_call = new List<EventAlterer>();
            alters_by_tag = new Dictionary<string, List<EventAlterer>>();
            alter_queue = new SimplePriorityQueue<EventAlterer>();

        }

        public Event(string id, string type, int priority)
        {
            this.id = id;
            this.type = type;
            this.priority = priority;
            parameters = new Dictionary<string, object>();
            tags = new HashSet<string> { type };
            priorities = new List<(Action<Event>, int)>();
            temp_entities = new Dictionary<string, Entity>();
            relevant_entities = new Dictionary<string, Entity>();
            triggered_events = new List<Event>();
            alters_to_call = new List<EventAlterer>();
            alters_by_tag = new Dictionary<string, List<EventAlterer>>();
            alter_queue = new SimplePriorityQueue<EventAlterer>();

        }

        public Event(JProperty p)
        {
            parameters = new Dictionary<string, object>();
            priorities = new List<(Action<Event>, int)>();
            id = p.Name;
            type = p.Value["type"] != null ? p.Value.Value<string>("type") : p.Name;
            priority = p.Value["priority"] != null ? p.Value.Value<int>("priority") : (1 << 30);
            temp_entities = new Dictionary<string, Entity>();
            relevant_entities = new Dictionary<string, Entity>();
            triggered_events = new List<Event>();
            alters_to_call = new List<EventAlterer>();
            alters_by_tag = new Dictionary<string, List<EventAlterer>>();
            alter_queue = new SimplePriorityQueue<EventAlterer>();

            tags = new HashSet<string> { type };
            if (p.Value["tags"] != null)
            {
                foreach (JToken tok in p.Value["tags"].ToList())
                {
                    tags.Add(tok.Value<string>());
                }
            }
        }

        public static Event NewEvent(object event_type)
        {
            return new Event(EventManager.instance.GetEvent(event_type.ToString()));
        }

        public static Event NewEvent(string event_type)
        {
            return new Event(EventManager.instance.GetEvent(event_type));
        }

        public static Event NewEvent(Event e, object event_type)
        {
            Event n_e = new Event(EventManager.instance.GetEvent(event_type.ToString()));
            n_e.AppendEvent(e);
            return n_e;
        }

        public static Event NewEvent(Event e, string event_type)
        {
            Event n_e = new Event(EventManager.instance.GetEvent(event_type));
            n_e.AppendEvent(e);
            return n_e;
        }

        public Event(Event el)
        {
            id = el.id;
            type = el.type;
            priorities = new List<(Action<Event>, int)>(el.priorities);
            parameters = new Dictionary<string, object>(el.parameters);
            tags = new HashSet<string>(el.tags);
            alters_to_call = new List<EventAlterer>(el.alters_to_call);
            alters_by_tag = new Dictionary<string, List<EventAlterer>>(el.alters_by_tag);
            alter_queue = new SimplePriorityQueue<EventAlterer>();

        }

        public Event(Event el, string forward_type)
        {
            parameters = new Dictionary<string, object>(el.parameters);
            priorities = new List<(Action<Event>, int)>(el.priorities);
            Event forward = EventManager.instance.GetEvent(forward_type);
            tags = new HashSet<string>(forward.tags);
            id = forward_type;
            type = forward.type;
            alters_to_call = new List<EventAlterer>(el.alters_to_call);
            alters_by_tag = new Dictionary<string, List<EventAlterer>>(el.alters_by_tag);
            alter_queue = new SimplePriorityQueue<EventAlterer>();

        }

        public Event(Event prior, Event next)
        {
            parameters = new Dictionary<string, object>(prior.parameters);
            foreach (KeyValuePair<string, object> ko in next.parameters)
            {
                SetParamValue(ko.Key, ko.Value, (k1, k2) => { return k1; });
            }
            priorities = new List<(Action<Event>, int)>(prior.priorities);
            priorities.AddRange(next.priorities);
            id = next.id;
            type = next.type;
            tags = new HashSet<string>(next.tags);
            alter_queue = new SimplePriorityQueue<EventAlterer>();

        }

        public bool TryGetTempEntity(string name, out Entity ent, bool create = false)
        {
            if (temp_entities.TryGetValue(name, out Entity val))
            {
                ent = val;
                return true;
            } else
            {
                ent = EntityManager.instance.CreateTempEntity(name);
                if (create)
                {
                    temp_entities.Add(name, ent);
                }
                return false;
            }
        }

        public bool AddTempEntity(string name, Entity ent, out Entity forward)
        {
            if (temp_entities.TryGetValue(name, out Entity val))
            {
                forward = val;
                return true;
            } else
            {
                forward = ent;
                temp_entities.Add(name, ent);
                return false;
            }
        }

        public bool TryGetRelevantEntity(string name, out Entity ent)
        {
            return relevant_entities.TryGetValue(name, out ent);
        }

        /// <summary>
        /// Adds entity to the relevant entities, returns a bool that says if an entity was already in the dictionary
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ent"></param>
        /// <returns></returns>
        public bool TryAddRelevantEntity(string name, Entity ent)
        {
            if (relevant_entities.ContainsKey(name))
            {
                relevant_entities[name] = ent;
                return true;
            } else
            {
                relevant_entities.Add(name, ent);
                return false;
            }
        }

        public List<Event> Invoke(Entity e)
        {
            List<Event> to_return = new List<Event> { this };
            SetParamValue("invoking_entity", e, (e1, e2) => { return e2; });
            Alter(e);
            SimplePriorityQueue<Action<Event>> temp_updates = new SimplePriorityQueue<Action<Event>>();
            foreach ((Action<Event>, int) ai in priorities)
            {
                temp_updates.Enqueue(ai.Item1, ai.Item2);
            }
            priorities = new List<(Action<Event>, int)>();
            while (temp_updates.Count > 0)
            {
                temp_updates.Dequeue()?.Invoke(this);
            }
            return to_return;
        }

        public void AlterInvoke()
        {
            foreach(EventAlterer alter in alters_to_call)
            {
                alter_queue.Enqueue(alter, alter.priority);
            }
            while(alter_queue.Count > 0)
            {
                alter_queue.Dequeue().Alter(this);
            }
        }

        public List<Event> Invoke()
        {
            SimplePriorityQueue<Action<Event>> temp_updates = new SimplePriorityQueue<Action<Event>>();
            foreach ((Action<Event>, int) ai in priorities)
            {
                temp_updates.Enqueue(ai.Item1, ai.Item2);
            }
            priorities = new List<(Action<Event>, int)>();
            while (temp_updates.Count > 0)
            {
                temp_updates.Dequeue()?.Invoke(this);
            }
            return triggered_events;
        }

        public T Invoke<T>(Entity e, string to_return)
        {
            Invoke(e);
            if (!parameters.TryGetValue(to_return, out object a))
            {
                Debug.LogError($"Could not find parameter {to_return} for event {id}");
                Debug.Log(ToString());
            }
            return (T)a;
        }

        public T Invoke<T>(Entity e, object to_return)
        {
            return Invoke<T>(e, to_return.ToString());
        }

        public void Alter(Entity e)
        {
            foreach (IBaseComponent b in e.components.Values)
            {
                b.Trigger(this);
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="child_id"></param>
        public void Cancel(string child_id = "")
        {
        }

        public void AddUpdate(Action<Event> a_e, int priority = 0)
        {
            priorities.Add((a_e, priority));
        }

        public void AddUpdate(EventAlterer alter)
        {
            alters_to_call.Add(alter);
            foreach (string s in alter.tags)
            {
                if (alters_by_tag.TryGetValue(s, out List<EventAlterer> alters))
                {
                    alters.Add(alter);
                } else
                {
                    alters_by_tag.Add(s, new List<EventAlterer> { alter });
                }
            }
        }

        public void BlockTag(string tag)
        {
            if(alters_by_tag.TryGetValue(tag, out List<EventAlterer> alters))
            {
                foreach(EventAlterer alter in alters)
                {
                    alters_to_call.Remove(alter);
                    alter_queue.Remove(alter);
                }
            }
        }

        public T GetParamValue<T>(string key)
        {
            if(!parameters.TryGetValue(key, out object to_return))
            {
                Debug.LogWarning($"Could not find Key {key} for parameters with event {id}");
                return default;
            }
            return (T)to_return;
        }

        public T GetParamValue<T>(object key)
        {
            return GetParamValue<T>(key.ToString());
        }

        public bool HasParamValue(string key)
        {
            return parameters.ContainsKey(key);
        }

        public bool HasParamValue(object key)
        {
            return HasParamValue(key.ToString());
        }

        public bool HasTag(object tag)
        {
            return tags.Contains(tag.ToString());
        }

        public void SetParamValue<T>(string key, T o, Func<T, T, T> combine_func)
        {
            if(parameters.TryGetValue(key, out object obj))
            {
                if (combine_func == null)
                {
                    Debug.LogWarning("An event component was called to change without a combiner function");
                    parameters[key] = o;
                }
                else
                {
                    parameters[key] = combine_func.Invoke((T)obj, o);
                }
            }
            else
            {
                parameters.Add(key, o);
            }
        }

        public void SetParamValue<T>(object key, T o, Func<T, T, T> combine_func)
        {
            SetParamValue(key.ToString(), o, combine_func);
        }

        public void AppendEvent(Event e)
        {
            tags.UnionWith(e.tags);
            foreach(KeyValuePair<string,object> pars in e.parameters)
            {
                SetParamValue(pars.Key, pars.Value, (o1,o2) => { return o2; });
            }
            foreach ((Action<Event>, int) cup in e.priorities)
            {
                AddUpdate(cup.Item1, cup.Item2);
            }
        }

        public override string ToString()
        {
            string s = $"id: {id},\ntags: [";
            int j = 0;
            foreach (string tag in tags)
            {
                s += $"{tag}";
                j += 1;
                if (j < tags.Count - 1)
                {
                    s += ", ";
                }
            }
            s += $"]\nParams: [";
            j = 0;
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

    public class EventAlterer
    {
        Action<Event> action;
        public int priority;
        public HashSet<string> tags;

        public EventAlterer()
        {
            tags = new HashSet<string>();
            priority = 0;
        }

        public EventAlterer(EventAlterer prot, int priority = -1)
        {
            action = prot.action;
            tags = new HashSet<string>(prot.tags);
            this.priority = priority >= 0 ? priority : prot.priority;
        }

        public EventAlterer(Action<Event> action, int priority, HashSet<string> tags = null)
        {
            this.action = action;
            this.priority = priority;
            this.tags = tags != null ? new HashSet<string>(tags) : new HashSet<string>();
        }

        public EventAlterer(Action<Event> action, int priority, string[] tags = null)
        {
            this.action = action;
            this.priority = priority;
            this.tags = tags != null ? new HashSet<string>(tags) : new HashSet<string>();
        }

        public void AddTag(string tag)
        {
            tags.Add(tag);
        }

        public void RemoveTag(string tag)
        {
            tags.Remove(tag);
        }

        public void Alter(Event e)
        {
            action(e);
        }
    }
    
}

/*
 * Idea on when an event is going to create a new event:
 *      Need function to make this not dumb to code
 *      SetParamValue(event_id, param_id, param_value, combine_func)? 
 *      Invoke(List<(entity_target, event_id)>)?
 */