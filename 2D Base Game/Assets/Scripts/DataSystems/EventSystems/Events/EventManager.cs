using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace HighKings
{
    public class EventManager : ITriggeredUpdater
    {
        public class TurnQueue
        {
            public Entity target;
            List<EventQueue> turns;

            public TurnQueue() { turns = new List<EventQueue>();  }
            public TurnQueue(Entity target)
            {
                this.target = target;
                turns = new List<EventQueue>();
            }

            /// <summary>
            /// Take the first turn off the queue
            /// </summary>
            /// <returns></returns>
            public EventQueue Pop()
            {
                while(turns.Count < 2)
                {
                    Enqueue(new EventQueue());
                }
                return Dequeue();
            }

            public EventQueue PopAndCall()
            {
                EventQueue q = Pop();
                q.Invoke(target);
                return q;
            }

            /// <summary>
            /// Add an event queue to the turns
            /// </summary>
            /// <param name="queue"></param>
            /// <param name="turns_forward"></param>
            public void Push(EventQueue queue, int turns_forward = 0)
            {
                while(turns_forward + 1 > turns.Count)
                {
                    Enqueue(new EventQueue());
                }
                turns[turns_forward].RegisterQueue(queue);
            }

            /// <summary>
            /// Push one event onto the event queue
            /// </summary>
            /// <param name="e"></param>
            /// <param name="turns_forward"></param>
            public void Push(Event e, int turns_forward = 0)
            {
                while(turns_forward + 1 > turns.Count)
                {
                    Enqueue(new EventQueue());
                }
                turns[turns_forward].RegisterEvent(e);
            }

            void Enqueue(EventQueue q)
            {
                turns.Add(q);
            }

            /// <summary>
            /// There are for sure faster ways to do this but for right now I don't care (thinking of maybe using cyclic indexing?) could just assume that events can only happen in a max length (maybe some x bits)
            /// and then do a mask on the index
            /// </summary>
            /// <returns></returns>
            EventQueue Dequeue()
            {
                EventQueue q = turns[0];
                turns.RemoveAt(0);
                return q;
            }
        }

        public static EventManager instance;
        public Dictionary<string, TurnQueue> turn_pairs;
        Dictionary<string, Event> event_prototypes;

        public EventManager()
        {
            if(instance != null)
            {
                Debug.LogWarning("There are at least two event managers trying to be made");
            } else
            {
                instance = this;
            }
            turn_pairs = new Dictionary<string, TurnQueue>();
            event_prototypes = new Dictionary<string, Event>();
        }

        public void Update()
        {
            List<TurnQueue> turns = (List<TurnQueue>)turn_pairs.Values.AsEnumerable();
            for(int i = turns.Count; i > 0; i -= 1)
            {
                turns[i - 1].PopAndCall();
            }
        }

        public void RemoveEntity(string id)
        {
            turn_pairs.Remove(id);
        }

        public void RemoveEntity(Entity e)
        {
            RemoveEntity(e.entity_string_id);
        }

        TurnQueue AddEntity(Entity e)
        {
            TurnQueue q = new TurnQueue(e);
            turn_pairs.Add(e.entity_string_id, q);
            return q;
        }

        TurnQueue AddEntity(string id)
        {
            TurnQueue q = new TurnQueue(EntityManager.instance.GetEntityFromId(id));
            turn_pairs.Add(id, q);
            return q;
        }

        public TurnQueue AddEvent(Entity target, EventQueue q, int turns_forward = 0)
        {
            if (turn_pairs.ContainsKey(target.entity_string_id))
            {
                TurnQueue t = turn_pairs[target.entity_string_id];
                t.Push(q, turns_forward);
                return t;
            } else
            {
                TurnQueue t = AddEntity(target);
                t.Push(q, turns_forward);
                return t;
            }
        }

        public TurnQueue AddEvent(string target, EventQueue q, int turns_forward = 0)
        {
            if (turn_pairs.ContainsKey(target))
            {
                TurnQueue t = turn_pairs[target];
                t.Push(q, turns_forward);
                return t;
            }
            else
            {
                TurnQueue t = AddEntity(target);
                t.Push(q, turns_forward);
                return t;
            }
        }

        public TurnQueue AddEvent(Entity target, Event q, int turns_forward = 0)
        {
            if (turn_pairs.ContainsKey(target.entity_string_id))
            {
                TurnQueue t = turn_pairs[target.entity_string_id];
                t.Push(q, turns_forward);
                return t;
            }
            else
            {
                TurnQueue t = AddEntity(target);
                t.Push(q, turns_forward);
                return t;
            }
        }

        public TurnQueue AddEvent(string target, Event q, int turns_forward = 0)
        {
            if (turn_pairs.ContainsKey(target))
            {
                TurnQueue t = turn_pairs[target];
                t.Push(q, turns_forward);
                return t;
            }
            else
            {
                TurnQueue t = AddEntity(target);
                t.Push(q, turns_forward);
                return t;
            }
        }

        public Event GetEvent(string id)
        {
            if (!event_prototypes.TryGetValue(id, out Event e))
            {
                Debug.LogWarning($"Could not find event {id}");
            }
            return e;
        }

        public Event PassEvent(Entity e, Event prot)
        {
            Event el = new Event(prot);
            el.AddUpdates(e);
            el.Invoke(e);
            return el;
        }

        public Event PassEvent(Entity e, string prot)
        {
            Event ev = GetEvent(prot);
            if(ev == null)
            {
                return null;
            }
            Event el = new Event(ev);
            el.AddUpdates(e);
            el.Invoke(e);
            return el;
        }
    }
}

