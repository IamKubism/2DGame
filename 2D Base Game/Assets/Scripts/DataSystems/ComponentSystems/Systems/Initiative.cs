using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Priority_Queue;

namespace Psingine
{
    /// <summary>
    /// TODO
    /// System that manages the cooldowns of all physical actions of entities, and calls their action queues. This is a generalization of the movement system to any action
    /// </summary>
    public class Initiatives : IUpdater
    {
        public enum InitiativeEvents { InitiativeUpdate }

        public class Initiative : IBaseComponent
        {
            public float value;
            public FloatMinMax curr;
            public SubscriberEvent subscriber { get; set; }

            public void CopyData(IBaseComponent comp)
            {
                throw new NotImplementedException();
            }

            public bool Trigger(Event e)
            {
                if (e.tags.Contains(InitiativeEvents.InitiativeUpdate.ToString()))
                {
                   
                }
                return true;
            }

        }

        Event initiative_update;
        Event turn_update;
        List<Entity> initiatives;
        SimplePriorityQueue<Entity> to_update;
        SimplePriorityQueue<Entity> next_update;


        public Initiatives()
        {
            initiatives = new List<Entity>();
            to_update = new SimplePriorityQueue<Entity>();
        }

        public void Update(float dt)
        {
            while(to_update.Count > 0)
            {
                Entity e = to_update.Dequeue();
                new Event(initiative_update).Invoke(e);
                Enqueue(next_update,e);
            }

            for (int i = initiatives.Count; i > 0; i -= 1)
            {
                (new Event(initiative_update)).Invoke(initiatives[i-1]);
            }

            while(next_update.Count > 0)
            {
                Enqueue(to_update, next_update.Dequeue());
            }
        }

        public void RemoveEntity(Entity e)
        {
            initiatives.Remove(e);
        }

        public void AddEntity(Entity e)
        {
            initiatives.Add(e);
        }

        void Enqueue(SimplePriorityQueue<Entity> qu,Entity e)
        {
            qu.Enqueue(e, -e.GetComponent<Initiative>().value);
        }
    }

}
