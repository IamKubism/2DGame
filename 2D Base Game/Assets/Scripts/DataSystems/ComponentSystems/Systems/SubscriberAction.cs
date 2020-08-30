using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings {

    public class SubscriberAction<T> where T: IBaseComponent
    {
        T comp;
        Dictionary<string, Action<T>> actions;

        public SubscriberAction(T t)
        {
            comp = t;
            actions = new Dictionary<string, Action<T>>();
        }

        public SubscriberAction(Entity e, string comp_name)
        {
            comp = e.GetComponent<T>(comp_name);
            actions = new Dictionary<string, Action<T>>();
        }
    
        public void SetAction(string action_name, Action<T> action)
        {
            if (actions.ContainsKey(action_name))
            {
                actions[action_name] = action;
            } else
            {
                actions.Add(action_name, action);
            }
        }

        public void RemoveAction(string action_name)
        {
            if (actions.ContainsKey(action_name))
            {
                actions.Remove(action_name);
            }
        }

        public void OperateOnComp()
        {
            foreach (Action<T> action in actions.Values)
            {
                action?.Invoke(comp);
            }
        }
    }
}