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
        
        public string component_name;

        Action<List<Entity>> on_entity_added;

        public ComponentSubscriber(string component_name)
        {
            this.component_name = component_name;
            after_actions = new Dictionary<Entity, SubscriberEvent<T>>();
            before_actions = new Dictionary<Entity, SubscriberEvent<T>>();
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

        public void RegisterOnAdded(Action<List<Entity>> to_reg)
        {
            on_entity_added += to_reg;
        }

        public void UnRegisterOnAdded(Action<List<Entity>> to_reg)
        {
            on_entity_added -= to_reg;
        }

        /// <summary>
        /// For when an event does not change a component
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="action"></param>
        /// <param name="action_name"></param>
        public void SubscribeAfterAction(List<Entity> entities, Action<Entity,T> action, string action_name)
        {
            foreach(Entity e in entities)
            {
                after_actions[e].RegisterAction(action_name, (t) => action(e, t));
            }
        }

        /// <summary>
        /// For when an event should change another component
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="entities"></param>
        /// <param name="action"></param>
        /// <param name="action_name"></param>
        public void SubscribeAfterAction<P>(List<Entity> entities, Action<Entity,T> action, string action_name, string p_name) where P: IBaseComponent
        {
            ComponentSubscriber<P> p_subscriber = MainGame.instance.GetSubscriberSystem<P>(p_name);
            foreach(Entity e in entities)
            {

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

        public string SysName()
        {
            return component_name;
        }

    }
}

