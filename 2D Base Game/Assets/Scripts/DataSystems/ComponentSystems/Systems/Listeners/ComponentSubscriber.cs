using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public class ComponentSubscriber<T> : ISystemAdder where T : IBaseComponent
    {
        /// <summary>
        /// The actions that run after this component is changed
        /// </summary>
        Dictionary<Entity, SubscriberEvent<T>> after_actions;
        Dictionary<Entity, SubscriberEvent<T>> before_actions;
        Dictionary<Entity, SubscriberEvent<T>> component_get_actions;
        
        public string component_name;

        Action<List<Entity>> on_entity_added;
        Action<List<Entity>> on_entity_removed;

        public ComponentSubscriber(string component_name)
        {
            this.component_name = component_name;
            after_actions = new Dictionary<Entity, SubscriberEvent<T>>();
            before_actions = new Dictionary<Entity, SubscriberEvent<T>>();
            component_get_actions = new Dictionary<Entity, SubscriberEvent<T>>();
            PrototypeLoader.instance.AddSystemLoc($"{component_name}_subscriber", this);
        }

        public void UpdateEntities(Dictionary<Entity, object[]> to_update, Action<Entity, object[]> action)
        {
            foreach(Entity t in to_update.Keys)
            {
                before_actions[t].OperateOnComp();
            }

            //Change all the components
            foreach(KeyValuePair<Entity, object[]> e in to_update)
            {
                action(e.Key, e.Value);
            }

            //Run all the subsribed updates
            foreach(Entity t in to_update.Keys)
            {
                after_actions[t].OperateOnComp();
            }
        }

        public void UpdateEntities(Dictionary<Entity, object[]> to_update, SubscriberEvent<T> action)
        {
            foreach (Entity t in to_update.Keys)
            {
                before_actions[t].OperateOnComp();
            }

            //Change all the components
            foreach (KeyValuePair<Entity, object[]> e in to_update)
            {
                action.OperateOnComp(e.Key.GetComponent<T>(component_name), e.Value);
            }

            //Run all the subsribed updates
            foreach (Entity t in to_update.Keys)
            {
                after_actions[t].OperateOnComp();
            }
        }

        public void UpdateOneEntity(Entity e, object[] args, SubscriberEvent<T> action)
        {
            before_actions[e].OperateOnComp();
            action.OperateOnComp(e.GetComponent<T>(component_name), args);
            after_actions[e].OperateOnComp();
        }

        public void UpdateComponents(Dictionary<Entity, object[]> to_update, Action<T, object[]> action)
        {
            Dictionary<T, object[]> comps = new Dictionary<T, object[]>();

            foreach (Entity t in to_update.Keys)
            {
                before_actions[t].OperateOnComp();
            }

            foreach (KeyValuePair<Entity,object[]> kv in to_update)
            {
                comps.Add(kv.Key.GetComponent<T>(component_name), kv.Value);
            }

            //Change all the components
            foreach (KeyValuePair<T, object[]> e in comps)
            {
                action.DynamicInvoke(e.Key, e.Value);
            }

            //Run all the subsribed updates
            foreach (Entity t in to_update.Keys)
            {
                after_actions[t].OperateOnComp();
            }
        }

        public void UpdateComponents(Dictionary<Entity, object[]> to_update, SubscriberEvent<T> action)
        {
            Dictionary<T, object[]> comps = new Dictionary<T, object[]>();

            foreach (Entity t in to_update.Keys)
            {
                before_actions[t].OperateOnComp();
            }

            foreach (KeyValuePair<Entity, object[]> kv in to_update)
            {
                comps.Add(kv.Key.GetComponent<T>(component_name), kv.Value);
            }

            foreach (KeyValuePair<T, object[]> e in comps)
            {
                action.OperateOnComp(e.Key, e.Value);
            }

            foreach (Entity t in to_update.Keys)
            {
                after_actions[t].OperateOnComp();
            }
        }

        public void AddEntities(List<Entity> entities)
        {
            foreach(Entity e in entities)
            {
                if(after_actions.ContainsKey(e) == false)
                {
                    after_actions.Add(e, new SubscriberEvent<T>(e, component_name));
                    before_actions.Add(e, new SubscriberEvent<T>(e, component_name));
                }
            }
            on_entity_added?.Invoke(entities);
        }

        public void RemoveEntities(List<Entity> entities)
        {
            foreach (Entity e in entities)
            {
                after_actions.Remove(e);
                before_actions.Remove(e);
            }
            on_entity_removed?.Invoke(entities);
        }

        public void RemoveEntity(Entity e)
        {
            if (after_actions.ContainsKey(e))
            {
                after_actions.Remove(e);
                before_actions.Remove(e);
            }
            on_entity_removed?.Invoke(new List<Entity> { e });
        }

        public void RegisterOnAdded(Action<List<Entity>> to_reg)
        {
            on_entity_added += to_reg;
        }

        public void UnRegisterOnAdded(Action<List<Entity>> to_reg)
        {
            on_entity_added -= to_reg;
        }

        public void RegisterOnRemove(Action<List<Entity>> to_reg)
        {
            on_entity_removed += to_reg;
        }

        public void UnRegisterOnRemove(Action<List<Entity>> to_reg)
        {
            on_entity_removed -= to_reg;
        }

        public void SubscribeAfterAction(List<Entity> entities, Action<Entity,T> action, string action_name)
        {
            foreach(Entity e in entities)
            {
                after_actions[e].RegisterAction(action_name, (t) => action(e, t));
            }
        }

        public void UnsubscribeAfterAction(List<Entity> components, string action)
        {
            foreach(Entity e in components)
            {
                after_actions[e].RemoveAction(action);
            }
        }

        public void SubscribeBeforeAction(List<Entity> entities, Action<Entity,T> action, string action_name)
        {
            foreach(Entity e in entities)
            {
                before_actions[e].RegisterAction(action_name, (t) => action(e,t));
            }
        }

        public void UnsubscribeBeforeAction(List<Entity> components, string action)
        {
            foreach(Entity e in components)
            {
                before_actions[e].RemoveAction(action);
            }
        }

        public void SubscribeGetAction(List<Entity> entities, Action<Entity, T> action, string action_name)
        {
            foreach (Entity e in entities)
            {
                component_get_actions[e].RegisterAction(action_name, (t) => action(e, t));
            }
        }

        public void UnsubscribeGetAction(List<Entity> components, string action)
        {
            foreach (Entity e in components)
            {
                component_get_actions[e].RemoveAction(action);
            }
        }

        public string SysCompName()
        {
            return component_name;
        }

    }
}

