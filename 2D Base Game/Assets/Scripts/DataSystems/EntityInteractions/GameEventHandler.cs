using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class GameEventHandler : IUpdater
    {
        public class EventQueue
        {
            public Entity target;
            public List<EntityEvent> discrete_events;
            public List<EntityEvent> continuous_events;
            public FloatMinMax continuous_time;

            public EventQueue()
            {
                discrete_events = new List<EntityEvent>();
                continuous_events = new List<EntityEvent>();
                continuous_time = new FloatMinMax();
            }

            public EventQueue(Entity target)
            {
                discrete_events = new List<EntityEvent>();
                continuous_time = new FloatMinMax();
                continuous_events = new List<EntityEvent>();
                this.target = target;
            }

            public void Update(float dt)
            {
                continuous_time += dt;
                continuous_events[0].Invoke(target);
                if (continuous_time.IsOverMax())
                {
                    discrete_events[0].Invoke(target);
                    discrete_events.RemoveAt(0);
                    continuous_events.RemoveAt(0);
                }
            }

            public void AddEvent(EntityEvent discrete_event, EntityEvent continuous_event, int turn)
            {

            }
        }

        public void Update(float dt)
        {
            throw new System.NotImplementedException();
        }
    }
}

