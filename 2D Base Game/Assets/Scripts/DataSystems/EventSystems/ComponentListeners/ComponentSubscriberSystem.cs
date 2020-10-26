using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public class ComponentSubscriberSystem: ISystemAdder
    {
        /// <summary>
        /// The actions that run after this component is changed
        /// </summary>
        Dictionary<Entity, SubscriberEvent> events;
        
        public string component_name;

        Action<List<Entity>> on_entities_added;
        Action<List<Entity>> on_entities_removed;

        public ComponentSubscriberSystem(string component_name)
        {
            this.component_name = component_name;
            events = new Dictionary<Entity, SubscriberEvent>();
            PrototypeLoader.instance.AddSystemLoc($"{component_name}_subscriber", this);
        }

        public void UpdateEntities(Dictionary<Entity, object[]> to_update, Action<Entity, object[]> action)
        {
            foreach(Entity t in to_update.Keys)
            {
                events[t].OperateBeforeOnComp();
            }

            //Change all the components
            foreach(KeyValuePair<Entity, object[]> e in to_update)
            {
                action(e.Key, e.Value);
            }

            //Run all the subsribed updates
            foreach(Entity t in to_update.Keys)
            {
                events[t].OperateAfterOnComp();
            }
        }

        public void UpdateEntities<T>(Dictionary<Entity, object[]> to_update, SubscriberEvent action) where T: IBaseComponent
        {
            foreach (Entity t in to_update.Keys)
            {
                events[t].OperateBeforeOnComp();
            }

            //Change all the components
            foreach (KeyValuePair<Entity, object[]> e in to_update)
            {
                action.OperateAllOnComp(e.Key.GetComponent<T>(component_name), e.Value);
            }

            //Run all the subsribed updates
            foreach (Entity t in to_update.Keys)
            {
                events[t].OperateAfterOnComp();
            }
        }

        public void UpdateComponents<T>(Dictionary<Entity, object[]> to_update, Action<T, object[]> action) where T : IBaseComponent
        {
            Dictionary<T, object[]> comps = new Dictionary<T, object[]>();

            foreach (Entity t in to_update.Keys)
            {
                events[t].OperateBeforeOnComp();
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
                events[t].OperateAfterOnComp();
            }
        }

        public void UpdateComponents(Dictionary<Entity, object[]> to_update, SubscriberEvent action)
        {
            Dictionary<IBaseComponent, object[]> comps = new Dictionary<IBaseComponent, object[]>();

            foreach (Entity t in to_update.Keys)
            {
                events[t].OperateBeforeOnComp();
            }

            foreach (KeyValuePair<Entity, object[]> kv in to_update)
            {
                comps.Add(kv.Key.GetComponent<IBaseComponent>(component_name), kv.Value);
            }

            foreach (KeyValuePair<IBaseComponent, object[]> e in comps)
            {
                action.OperateAllOnComp(e.Key, e.Value);
            }

            foreach (Entity t in to_update.Keys)
            {
                events[t].OperateAfterOnComp();
            }
        }

        public void AddEntities(List<Entity> entities)
        {
            foreach(Entity e in entities)
            {
                if(events.ContainsKey(e) == false)
                {
                    events.Add(e, new SubscriberEvent(e, component_name));
                }
            }
            on_entities_added?.Invoke(entities);
        }

        public void RemoveEntities(List<Entity> entities)
        {
            foreach (Entity e in entities)
            {
                events.Remove(e);
            }
            on_entities_removed?.Invoke(entities);
        }

        public void RemoveEntity(Entity e)
        {
            if (events.ContainsKey(e))
            {
                events.Remove(e);
            }
            on_entities_removed?.Invoke(new List<Entity> { e });
        }

        public void RegisterOnAdded(Action<List<Entity>> to_reg)
        {
            on_entities_added += to_reg;
        }

        public void UnRegisterOnAdded(Action<List<Entity>> to_reg)
        {
            on_entities_added -= to_reg;
        }

        public void RegisterOnRemove(Action<List<Entity>> to_reg)
        {
            on_entities_removed += to_reg;
        }

        public void UnRegisterOnRemove(Action<List<Entity>> to_reg)
        {
            on_entities_removed -= to_reg;
        }

        public void SubscribeAfterAction(List<Entity> entities, Action<Entity,IBaseComponent> action, string action_name)
        {
            foreach(Entity e in entities)
            {
                events[e].RegisterAfterAction(action_name, (t) => action(e, t));
            }
        }

        public void SubscribeAfterAction<T>(List<Entity> entities, Action<Entity,T> action, string action_name)
        {
            foreach(Entity e in entities)
            {
                events[e].RegisterAfterAction(action_name, (t) => action(e, (T)t));
            }
        }

        public void UnsubscribeAfterAction(List<Entity> components, string action)
        {
            foreach(Entity e in components)
            {
                if(events.ContainsKey(e))
                    events[e].RemoveAfterAction(action);
            }
        }

        public void UnsubscribeAfterAction(Entity entity, string action)
        {
            if(events.ContainsKey(entity))
                events[entity].RemoveAfterAction(action);
        }

        public void UnsubscribeAfterAction(string entity, string action)
        {
            if(EntityManager.instance.GetEntityFromId(entity) != null)
                if(events.ContainsKey(EntityManager.instance.GetEntityFromId(entity)))
                    events[EntityManager.instance.GetEntityFromId(entity)].RemoveAfterAction(action);
        }

        public void SubscribeBeforeAction(List<Entity> entities, Action<Entity,IBaseComponent> action, string action_name)
        {
            foreach(Entity e in entities)
            {
                events[e].RegisterBeforeAction(action_name, (t) => action(e,t));
            }
        }

        public void SubscribeBeforeAction<T>(List<Entity> entities, Action<Entity,T> action, string action_name)
        {
            foreach(Entity e in entities)
            {
                events[e].RegisterBeforeAction(action_name, (t) => action(e,(T)t));
            }
        }

        public void UnsubscribeBeforeAction(List<Entity> components, string action)
        {
            foreach(Entity e in components)
            {
                events[e].RemoveBeforeAction(action);
            }
        }


        public string SysCompName()
        {
            return component_name;
        }

    }
}

