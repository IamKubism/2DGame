using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public class ComponentSubscriber<T> : ISystemAdder where T : IBaseComponent
    {
        /// <summary>
        /// The actions that run when this component is changed
        /// </summary>
        Dictionary<Entity, Action<Entity>> subscribed_actions;
        public string component_name;

        public ComponentSubscriber(string component_name)
        {
            this.component_name = component_name;
            subscribed_actions = new Dictionary<Entity, Action<Entity>>();
            PrototypeLoader.instance.AddSystemLoc($"{component_name}_updater", this);
        }

        public void UpdateEntities(Dictionary<Entity, object[]> to_update, Action<Entity, object[]> action)
        {
            //Change all the components
            foreach(KeyValuePair<Entity, object[]> e in to_update)
            {
                action(e.Key, e.Value);
            }

            //Run all the subsribed updates
            foreach(Entity t in to_update.Keys)
            {
                subscribed_actions[t](t);
            }
        }

        public void AddEntities(List<Entity> entities)
        {
            foreach(Entity e in entities)
            {
                if (subscribed_actions.ContainsKey(e) == false)
                {
                    subscribed_actions.Add(e, null);
                }
            }
        }

        public void OnAddedEntities(List<Entity> entities)
        {
            throw new System.NotImplementedException();
        }

        public void SubscribeAction(List<Entity> entities, Action<Entity> action)
        {
            foreach(Entity e in entities)
            {
                subscribed_actions[e] += action;
            }
        }

        public void UnsubscribeAction(List<Entity> components, Action<Entity> action)
        {
            foreach(Entity e in components)
            {
                subscribed_actions[e] -= action;
            }
        }
    }
}

