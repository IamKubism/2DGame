using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace HighKings
{
    public class EntityEvent
    {
        Dictionary<string, EventListener> listeners;
        List<EventListener> sorted_call_list;
        public string action_id { get; protected set; }
        public string retrieval_action;

        public EntityEvent(string action_id, Dictionary<string, EventListener> listeners)
        {
            this.listeners = new Dictionary<string, EventListener>(listeners);
            sorted_call_list = new List<EventListener>();
            this.action_id = action_id;
        }

        public EntityEvent(string action_id)
        {
            listeners = new Dictionary<string, EventListener>();
            sorted_call_list = new List<EventListener>();
            this.action_id = action_id;
        }

        public EntityEvent(JProperty prop)
        {
            listeners = new Dictionary<string, EventListener>();
            sorted_call_list = new List<EventListener>();
            action_id = prop.Name;
            List<JToken> toks = prop.Value["listeners"].ToList();
            foreach (JProperty p in toks)
            {
                RegisterListener(new EventListener(p));
            }
            retrieval_action = prop.Value["retrieval_action"].Value<string>();
        }

        public EntityEvent(string action_id, string retrieval_action, EventListener listener)
        {
            listeners = new Dictionary<string, EventListener> { { listener.id, listener } };
            sorted_call_list = new List<EventListener>
            {
                listener
            };
            this.retrieval_action = retrieval_action;
            this.action_id = action_id;
        }

        public EntityEvent(string action_id, EntityEvent action)
        {
            listeners = new Dictionary<string, EventListener>(action.listeners);
            sorted_call_list = new List<EventListener>(action.sorted_call_list);
            this.action_id = action_id;
        }

        public EntityEvent(EntityEvent action)
        {
            listeners = new Dictionary<string, EventListener>(action.listeners);
            sorted_call_list = new List<EventListener>(action.sorted_call_list);
            action_id = action.action_id;
        }

        public void SetRetrievalAction(string retrieval_action)
        {
            this.retrieval_action = retrieval_action;
        }

        public void AppendAction(EntityEvent action)
        {
            foreach (EventListener el in action.listeners.Values)
            {
                if (listeners.ContainsKey(el.id) == false)
                {
                    listeners.Add(el.id, el);
                }
            }
        }

        public void Invoke(Entity target)
        {
            for (int i = listeners.Count; i > 0; i -= 1)
            {
                sorted_call_list[i - 1].Invoke(target);
            }
        }

        public void RegisterListener(EventListener listener)
        {
            if (listeners.ContainsKey(listener.id))
            {
                Debug.LogError("Already had listener of name: " + listener.id);
            }
            else
            {
                listeners.Add(listener.id, listener);
                for (int i = 0; i < sorted_call_list.Count; i += 1)
                {
                    if (sorted_call_list[i].priority <= listener.priority)
                    {
                        sorted_call_list.Insert(i, listener);
                        continue;
                    }
                }
                sorted_call_list.Add(listener);
            }
        }

        public void CreateSortList(List<EventListener> listeners)
        {
            sorted_call_list = new List<EventListener>();
            foreach (EventListener listener in listeners)
            {
                for (int i = 0; i < sorted_call_list.Count; i += 1)
                {
                    if (sorted_call_list[i].priority >= listener.priority)
                    {
                        sorted_call_list.Insert(i, listener);
                        continue;
                    }
                }
                sorted_call_list.Add(listener);
            }
        }

        public void UnRegisterListener(string id)
        {
            if (listeners.ContainsKey(id))
            {
                sorted_call_list.Remove(listeners[id]);
                listeners.Remove(id);
            }
        }

        public int GetListenerPriority(string s)
        {
            if (listeners.ContainsKey(s))
            {
                return listeners[s].priority;
            }
            else
            {
                return 100;
            }
        }

        /// <summary>
        /// Creates a unioned action (calls a unioned set of the listeners of a and b)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static EntityEvent operator +(EntityEvent a, EntityEvent b)
        {
            EntityEvent c = new EntityEvent(a);
            foreach (EventListener listener in b.listeners.Values)
            {
                if (c.listeners.ContainsKey(listener.id))
                {
                    continue;
                }
                c.RegisterListener(listener);
            }

            return c;
        }

    }
}