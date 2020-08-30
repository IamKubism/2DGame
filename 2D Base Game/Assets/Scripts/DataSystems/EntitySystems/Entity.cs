using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings {

    public class Entity
    {
        public readonly int entity_id;
        public readonly string entity_string_id;
        Dictionary<string, IBaseComponent> components;

        public Entity(string entity_string_id, int entity_id)
        {
            this.entity_id = entity_id;
            this.entity_string_id = entity_string_id;
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

        public void RemoveComponent(string id)
        {
            if (components.ContainsKey(id))
            {
                components.Remove(id);
            } else
            {
                Debug.LogError($"Tried to remove a component of type {id} on {entity_string_id} when none exists");
            }
        }

        public bool HasComponent(string type)
        {
            return components.ContainsKey(type);
        }

        public T GetComponent<T>(string comp_name) where T: IBaseComponent
        {
            return components.ContainsKey(comp_name) ? (T)components[comp_name] : default;
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

