﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace Psingine {

    public class Entity
    {
        public readonly int entity_id;
        public readonly string entity_string_id;
        public string entity_type;
        public Dictionary<string, IBaseComponent> components { get; protected set; }

        public Entity(string entity_string_id, int entity_id)
        {
            this.entity_id = entity_id;
            this.entity_string_id = entity_string_id;
            components = new Dictionary<string, IBaseComponent>();
        }

        public Entity CopyEntity(string string_id)
        {
            return PrototypeLoader.instance.CopyEntity(this, string_id);
        }

        public IBaseComponent TryAddComponent(string id, IBaseComponent comp)
        {
            if (components.TryGetValue(id, out IBaseComponent component))
            {
                return component;
            }
            else
            {
                components.Add(id, comp);
                return comp;
            }
        }

        public IBaseComponent FullSetComponent(string id, IBaseComponent comp)
        {
            if(components.TryGetValue(id, out IBaseComponent val))
            {
                val.CopyData(comp);
            } else
            {
                components.Add(id, comp);
                MainGame.instance.GetSubscriberSystem(id).AddEntity(this);
            }
            return comp;
        }

        public void RemoveComponent(string id)
        {
            if (components.ContainsKey(id))
            {
                components.Remove(id);
            }
            else
            {
                Debug.LogWarning($"Tried to remove a component of type {id} on {entity_string_id} when none exists");
            }
        }

        public bool HasComponent(string type)
        {
            return components.ContainsKey(type);
        }

        public T GetComponent<T>(string comp_name) where T : IBaseComponent
        {
            if (!components.TryGetValue(comp_name, out IBaseComponent bc))
            {
                Debug.LogWarning($"Could not find component {comp_name} for entity {entity_string_id}");
            }
            return (T)bc;
        }

        public bool TryGetComponent<T>(string comp_name, out T comp) where T : IBaseComponent
        {
            if (components.TryGetValue(comp_name, out IBaseComponent to_return))
            {
                comp = (T)to_return;
                return true;
            }
            else
            {
                comp = default;
                return false;
            }
        }

        public bool TryGetComponent<T>(out T comp) where T : IBaseComponent
        {
            if (components.TryGetValue(nameof(T), out IBaseComponent to_return))
            {
                comp = (T)to_return;
                return true;
            }
            else
            {
                comp = default;
                return false;
            }
        }

        public IBaseComponent GetComponent(string comp_name)
        {
            if (!components.TryGetValue(comp_name, out IBaseComponent bc))
            {
                Debug.LogWarning($"Could not find component {comp_name} for entity {entity_string_id}");
            }
            return bc;
        }

        public T GetComponent<T>() where T : IBaseComponent
        {
            if (!components.TryGetValue(typeof(T).Name, out IBaseComponent b))
            {
                Debug.LogWarning($"Could not find component {typeof(T).Name} for entity {entity_string_id}");
            }
            return (T)b;
        }

        public override bool Equals(object obj)
        {
            return entity_string_id == ((Entity)obj).entity_string_id;
        }

        public override string ToString()
        {
            string s = entity_string_id;
            if (components.Count == 0)
            {
                s += "No components";
            }
            foreach (KeyValuePair<string, IBaseComponent> ko in components)
            {
                s += $"\n {ko.Key}: {ko.Value.ToString()}";
            }
            return s;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void AssignEvent(Event e, int turns_forward = 0)
        {
            eventManager().AddEvent(this, e, turns_forward);
        }

        /// <summary>
        /// TODO
        /// </summary>
        public void RemoveFromAllSubscribers()
        {
            //Type subscriber_type = typeof(ComponentSubscriberSystem<>);
            //MethodInfo get_sub_method = MainGame.instance.GetType().GetMethod("GetSubscriberSystem", new Type[1] { typeof(string) });
            //foreach (KeyValuePair<string, IBaseComponent> comps in components)
            //{
            //    MethodInfo remove_method = subscriber_type.MakeGenericType(new Type[1] { comps.Value.GetType() }).GetMethod("RemoveEntity");
            //    object subs = get_sub_method.MakeGenericMethod(comps.Value.GetType()).Invoke(
            //            MainGame.instance,
            //            new object[1] { comps.Key });
            //    remove_method.Invoke(subs, new object[1] { this });
            //    //Debug.Log($"Removed {entity_string_id} from {comps.Key}");
            //}
        }

        public static EntityManager Manager()
        {
            return EntityManager.instance;
        }

        public static EventManager eventManager()
        {
            return EventManager.instance;
        }

        public static void Destroy(Entity e, bool temp = false)
        {
            if (!temp)
            {
                e.RemoveFromAllSubscribers();
            }
            Manager().DestroyEntity(e);
        }

        public static bool Exists(Entity e)
        {
            return Manager().CheckExistance(e);
        }

        public static implicit operator Entity(string id)
        {
            if (Manager().CheckExistance(id))
            {
                return Manager()[id];
            } else
            {
                return null;
            }
        }

        public static implicit operator string(Entity e)
        {
            return e.entity_string_id;
        }
    }
}

