using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace HighKings
{
    /// <summary>
    /// A collection of listeners that will each be invoked when this is invoked. These are functions returning nothing and taking source and target entities as arguments
    /// </summary>
    public class EntityAction
    {
        Dictionary<string, EntityListener> listeners;
        List<EntityListener> sorted_call_list;
        public string action_id { get; protected set; }
        public List<string> source_tags;
        public List<string> target_tags;
        public string retrieval_action;

        public EntityAction(string action_id, Dictionary<string, EntityListener> listeners)
        {
            this.listeners = new Dictionary<string, EntityListener>(listeners);
            sorted_call_list = new List<EntityListener>();
            this.action_id = action_id;
        }

        public EntityAction(string action_id)
        {
            listeners = new Dictionary<string, EntityListener>();
            sorted_call_list = new List<EntityListener>();
            this.action_id = action_id;
        }

        public EntityAction(JProperty prop)
        {
            listeners = new Dictionary<string, EntityListener>();
            sorted_call_list = new List<EntityListener>();
            action_id = prop.Name;
            List<JToken> toks = prop.Value["listeners"].ToList();
            foreach(JProperty p in toks)
            {
                RegisterListener(new EntityListener(p));
            }
            retrieval_action = prop.Value["retrieval_action"].Value<string>();
        }

        public EntityAction(string action_id, string retrieval_action, EntityListener listener)
        {
            listeners = new Dictionary<string, EntityListener> { { listener.id, listener } };
            sorted_call_list = new List<EntityListener>
            {
                listener
            };
            this.retrieval_action = retrieval_action;
            this.action_id = action_id;
        }

        public EntityAction(string action_id, EntityAction action)
        {
            listeners = new Dictionary<string, EntityListener>(action.listeners);
            sorted_call_list = new List<EntityListener>(action.sorted_call_list);
            this.action_id = action_id;
        }

        public void SetRetrievalAction(string retrieval_action)
        {
            this.retrieval_action = retrieval_action;
        }

        public void AppendAction(EntityAction action)
        {
            foreach(EntityListener el in action.listeners.Values)
            {
                if (listeners.ContainsKey(el.id) == false)
                {
                    listeners.Add(el.id, el);
                }
            }
        }

        public void Invoke(Entity source, Entity target)
        {
            for(int i = sorted_call_list.Count; i > 0; i -= 1)
            {
                sorted_call_list[i-1].Invoke(source, target);
            }
        }

        public void RegisterListener(EntityListener listener)
        {
            if (listeners.ContainsKey(listener.id))
            {
                Debug.LogError("Already had listener of name: " + listener.id);
            } else
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

        public void CreateSortList(List<EntityListener> listeners)
        {
            sorted_call_list = new List<EntityListener>();
            foreach (EntityListener listener in listeners)
            {
                for(int i = 0; i < sorted_call_list.Count; i += 1)
                {
                    if(sorted_call_list[i].priority <= listener.priority)
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

    }
}

