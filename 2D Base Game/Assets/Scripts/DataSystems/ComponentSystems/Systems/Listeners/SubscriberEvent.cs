using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public class SubscriberEvent<T> where T: IBaseComponent
    {
        T comp;
        Dictionary<string, ComponentListener<T>> actions;
        List<ComponentListener<T>> action_list;

        public SubscriberEvent(T t)
        {
            comp = t;
            actions = new Dictionary<string, ComponentListener<T>>();
            action_list = new List<ComponentListener<T>>();
        }

        public SubscriberEvent(Entity e, string comp_name)
        {
            comp = e.GetComponent<T>(comp_name);
            actions = new Dictionary<string, ComponentListener<T>>();
            action_list = new List<ComponentListener<T>>();
        }
    
        public void RegisterAction(string action_name, ComponentListener<T> action)
        {
            if (actions.ContainsKey(action_name))
            {
                actions[action_name] = action;
            } else
            {
                actions.Add(action_name, action);
                action_list.Add(action);
            }
        }

        public void RegisterAction(string action_name, Action<T> action)
        {
            if (actions.ContainsKey(action_name))
            {
                actions[action_name].ResetAction(action);
            }
            else
            {
                ComponentListener<T> listener = new ComponentListener<T>(action_name, action);
                actions.Add(action_name, listener);
                action_list.Add(listener);
            }
        }

        public void RemoveAction(string action_name)
        {
            if (actions.ContainsKey(action_name))
            {
                action_list.Remove(actions[action_name]);
                actions.Remove(action_name);
            }
        }

        public void OperateOnComp()
        {
            for(int i = action_list.Count; i > 0; i -= 1)
            {
                action_list[i - 1].Invoke(comp);
            }
        }

        public void OperateOnComp(T t)
        {
            for (int i = action_list.Count; i > 0; i -= 1)
            {
                action_list[i - 1].Invoke(comp);
            }
        }

        public void OperateOnComp(T t, object[] args)
        {
            for (int i = action_list.Count; i > 0; i -= 1)
            {
                action_list[i - 1].Invoke(comp, args);
            }
        }
    }
}