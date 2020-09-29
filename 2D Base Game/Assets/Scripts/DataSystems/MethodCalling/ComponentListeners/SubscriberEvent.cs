using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public class SubscriberEvent<T> where T: IBaseComponent
    {
        T comp;
        Dictionary<string, ComponentListener<T>> before_actions;
        Dictionary<string, ComponentListener<T>> after_actions;
        List<ComponentListener<T>> before_action_list;
        List<ComponentListener<T>> after_action_list;

        public SubscriberEvent(T t)
        {
            comp = t;
            before_actions = new Dictionary<string, ComponentListener<T>>();
            after_actions = new Dictionary<string, ComponentListener<T>>();
            before_action_list = new List<ComponentListener<T>>();
            after_action_list = new List<ComponentListener<T>>();
            comp.SetListener(this);
        }

        public SubscriberEvent(Entity e, string comp_name)
        {
            comp = e.GetComponent<T>(comp_name);
            before_actions = new Dictionary<string, ComponentListener<T>>();
            after_actions = new Dictionary<string, ComponentListener<T>>();
            before_action_list = new List<ComponentListener<T>>();
            after_action_list = new List<ComponentListener<T>>();
            comp.SetListener(this);
        }

        public void RegisterBeforeAction(string action_name, ComponentListener<T> action)
        {
            if (before_actions.ContainsKey(action_name))
            {
                before_actions[action_name] = action;
            } else
            {
                before_actions.Add(action_name, action);
                before_action_list.Add(action);
            }
        }

        public void RegisterBeforeAction(string action_name, Action<T> action)
        {
            if (before_actions.ContainsKey(action_name))
            {
                before_actions[action_name].ResetAction(action);
            }
            else
            {
                ComponentListener<T> listener = new ComponentListener<T>(action_name, action);
                before_actions.Add(action_name, listener);
                before_action_list.Add(listener);
            }
        }

        public void RemoveBeforeAction(string action_name)
        {
            if (before_actions.ContainsKey(action_name))
            {
                before_action_list.Remove(before_actions[action_name]);
                before_actions.Remove(action_name);
            }
        }

        public void RegisterAfterAction(string action_name, ComponentListener<T> action)
        {
            if (after_actions.ContainsKey(action_name))
            {
                after_actions[action_name] = action;
            }
            else
            {
                after_actions.Add(action_name, action);
                after_action_list.Add(action);
            }
        }

        public void RegisterAfterAction(string action_name, Action<T> action)
        {
            if (after_actions.ContainsKey(action_name))
            {
                after_actions[action_name].ResetAction(action);
            }
            else
            {
                ComponentListener<T> listener = new ComponentListener<T>(action_name, action);
                after_actions.Add(action_name, listener);
                after_action_list.Add(listener);
            }
        }

        public void RemoveAfterAction(string action_name)
        {
            if (after_actions.ContainsKey(action_name))
            {
                after_action_list.Remove(after_actions[action_name]);
                after_actions.Remove(action_name);
            }
        }

        public void OperateBeforeOnComp()
        {
            for(int i = before_action_list.Count; i > 0; i -= 1)
            {
                before_action_list[i - 1].Invoke(comp);
            }
        }

        public void OperateBeforeOnComp(T t)
        {
            for (int i = before_action_list.Count; i > 0; i -= 1)
            {
                before_action_list[i - 1].Invoke(t);
            }
        }

        public void OperateBeforeOnComp(T t, object[] args)
        {
            for (int i = before_action_list.Count; i > 0; i -= 1)
            {
                before_action_list[i - 1].Invoke(t, args);
            }
        }

        public void OperateAfterOnComp()
        {
            for (int i = after_action_list.Count; i > 0; i -= 1)
            {
                after_action_list[i - 1].Invoke(comp);
            }
        }

        public void OperateAfterOnComp(T t)
        {
            for (int i = after_action_list.Count; i > 0; i -= 1)
            {
                after_action_list[i - 1].Invoke(t);
            }
        }

        public void OperateAfterOnComp(T t, object[] args)
        {
            for (int i = after_action_list.Count; i > 0; i -= 1)
            {
                after_action_list[i - 1].Invoke(t, args);
            }
        }

        public void OperateAllOnComp()
        {
            for (int i = before_action_list.Count; i > 0; i -= 1)
            {
                before_action_list[i - 1].Invoke(comp);
            }
            for (int i = after_action_list.Count; i > 0; i -= 1)
            {
                after_action_list[i - 1].Invoke(comp);
            }
        }

        public void OperateAllOnComp(T t)
        {
            for (int i = before_action_list.Count; i > 0; i -= 1)
            {
                before_action_list[i - 1].Invoke(t);
            }
            for (int i = after_action_list.Count; i > 0; i -= 1)
            {
                after_action_list[i - 1].Invoke(comp);
            }
        }

        public void OperateAllOnComp(T t, object[] args)
        {
            for (int i = before_action_list.Count; i > 0; i -= 1)
            {
                before_action_list[i - 1].Invoke(t, args);
            }
            for (int i = after_action_list.Count; i > 0; i -= 1)
            {
                after_action_list[i - 1].Invoke(comp, args);
            }
        }
    }
}