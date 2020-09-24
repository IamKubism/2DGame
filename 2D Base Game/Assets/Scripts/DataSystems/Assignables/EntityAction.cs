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
        public string action_id { get; protected set; }

        public EntityAction(string action_id, Dictionary<string, EntityListener> listeners)
        {
            this.listeners = new Dictionary<string, EntityListener>(listeners);
            this.action_id = action_id;
        }

        public EntityAction(string action_id)
        {
            listeners = new Dictionary<string, EntityListener>();
            this.action_id = action_id;
        }

        public EntityAction(JProperty prop)
        {
            listeners = new Dictionary<string, EntityListener>();
            action_id = prop.Name;
            List<JToken> toks = prop.Value["listeners"].ToList();
            foreach(JProperty p in toks)
            {
                RegisterAction(new EntityListener(p));
            }
        }

        public EntityAction(string action_id, EntityListener listener)
        {
            this.listeners = new Dictionary<string, EntityListener> { { listener.id, listener } };
            this.action_id = action_id;
        }

        public void Invoke(Entity source, Entity target)
        {
            List<string> ks = listeners.Keys.ToList();
            for(int i = listeners.Count; i > 0; i -= 1)
            {
                listeners[ks[i - 1]].Invoke(source, target);
            }
        }

        public void RegisterAction(EntityListener listener)
        {
            if (listeners.ContainsKey(listener.id))
            {
                Debug.LogError("Already had listener of name: " + listener.id);
            } else
            {
                listeners.Add(listener.id, listener);
            }
        }


        public void UnRegisterListener(string id)
        {
            if (listeners.ContainsKey(id))
            {
                listeners.Remove(id);
            }
        }

    }
}

