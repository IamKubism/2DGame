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
            public List<HighKings.EventQueue> discrete_events;
            public List<HighKings.EventQueue> continuous_events;
            public FloatMinMax continuous_time;

            public EventQueue()
            {
                discrete_events = new List<HighKings.EventQueue>();
                continuous_events = new List<HighKings.EventQueue>();
                continuous_time = new FloatMinMax();
            }

            public EventQueue(Entity target)
            {
                discrete_events = new List<HighKings.EventQueue>();
                continuous_time = new FloatMinMax();
                continuous_events = new List<HighKings.EventQueue>();
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

            public void AddEvent(HighKings.EventQueue discrete_event, HighKings.EventQueue continuous_event, int turn)
            {

            }
        }

        public void Update(float dt)
        {
            throw new System.NotImplementedException();
        }
    }
}

