using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings {

    public class Entity
    {
        public readonly string entity_string_id;
        public Dictionary<string, IBaseComponent> components;

        public Entity(string id)
        {
            entity_string_id = id;
            components = new Dictionary<string, IBaseComponent>();
        }

        public void AddComponent(string id, IBaseComponent comp)
        {
            if (components.ContainsKey(id))
            {
                Debug.Log($"Tried to set a component of type {id} twice on {entity_string_id}");
            }
            else
            {
                components.Add(id, comp);
            }
        }

        public T GetComponent<T>(string type)
        {
            return components.ContainsKey(type) ? (T)components[type] : default;
        }

        public override bool Equals(object obj)
        {
            
            return entity_string_id == ((Entity)obj).entity_string_id;
        }

        public override string ToString()
        {
            string s = entity_string_id;
            if(components.Count == 0)
            {
                s += "No components";
            }
            foreach(KeyValuePair<string,IBaseComponent> ko in components)
            {
                s += $"\n {ko.Key}: {ko.Value.ToString()}";
            }
            return s;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

