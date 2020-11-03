using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace HighKings
{
    public class EventManager : ITriggeredUpdater
    {
        public class Turns
        {
            public Entity target;
            List<EventQueue> turns;

            public Turns() { turns = new List<EventQueue>();  }

            public Turns(Entity target)
            {
                this.target = target;
                turns = new List<EventQueue>();
            }

            public Turns(Entity target, Event e)
            {
                this.target = target;
                turns = new List<EventQueue>();
                Push(e);
            }

            /// <summary>
            /// Take the first turn off the queue
            /// </summary>
            /// <returns></returns>
            public EventQueue Pop()
            {
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
            /// <param name="turn"></param>
            public void Push(EventQueue queue, int turn = 0)
            {
                while(turn > turns.Count)
                {
                    Enqueue(new EventQueue());
                }
                turns[turn].RegisterQueue(queue);
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

            EventQueue Dequeue()
            {
                EventQueue q = turns[0];
                turns.RemoveAt(0);
                return q;
            }
        }

        public static EventManager instance;
        public Dictionary<string, Turns> turn_pairs;
        Dictionary<string, Event> event_prototypes;
        List<Entity> continuous_updaters;

        public EventManager()
        {
            if(instance != null)
            {
                Debug.LogWarning("There are at least two event managers trying to be made");
            } else
            {
                instance = this;
            }
            turn_pairs = new Dictionary<string, Turns>();
            event_prototypes = new Dictionary<string, Event>();
            continuous_updaters = new List<Entity>();
        }

        public void Update()
        {
            List<Turns> turns = (List<Turns>)turn_pairs.Values.AsEnumerable();
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

        Turns AddEntity(Entity e)
        {
            Turns q = new Turns(e);
            turn_pairs.Add(e.entity_string_id, q);
            return q;
        }

        Turns AddEntity(string id)
        {
            Turns q = new Turns(EntityManager.instance.GetEntityFromId(id));
            turn_pairs.Add(id, q);
            return q;
        }

        public Turns AddEvent(Entity target, EventQueue q, int turns_forward = 0)
        {
            if (turn_pairs.ContainsKey(target.entity_string_id))
            {
                Turns t = turn_pairs[target.entity_string_id];
                t.Push(q, turns_forward);
                return t;
            } else
            {
                Turns t = AddEntity(target);
                t.Push(q, turns_forward);
                return t;
            }
        }

        public Turns AddEvent(string target, EventQueue q, int turns_forward = 0)
        {
            if (turn_pairs.ContainsKey(target))
            {
                Turns t = turn_pairs[target];
                t.Push(q, turns_forward);
                return t;
            }
            else
            {
                Turns t = AddEntity(target);
                t.Push(q, turns_forward);
                return t;
            }
        }

        public Turns AddEvent(Entity target, Event q, int turns_forward = 0)
        {
            if (turn_pairs.ContainsKey(target.entity_string_id))
            {
                Turns t = turn_pairs[target.entity_string_id];
                t.Push(q, turns_forward);
                return t;
            }
            else
            {
                Turns t = AddEntity(target);
                t.Push(q, turns_forward);
                return t;
            }
        }

        public Turns AddEvent(string target, Event q, int turns_forward = 0)
        {
            if (turn_pairs.ContainsKey(target))
            {
                Turns t = turn_pairs[target];
                t.Push(q, turns_forward);
                return t;
            }
            else
            {
                Turns t = AddEntity(target);
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

        public void AddEventPrototype(JProperty p)
        {
            event_prototypes.Add(p.Name, new Event(p));
        }

        public Event PushEventToQueue(Entity entity, Event e, int turn = 0)
        {
            Event el = new Event(e);
            if (!turn_pairs.ContainsKey(entity))
            {
                turn_pairs.Add(entity, new Turns(entity, el));
            } else
            {
                turn_pairs[entity].Push(el, turn);
            }
            return el;
        }

        public Event PushEventToQueue(Entity entity, string prot, int turn = 0)
        {
            if(GetEvent(prot) == null)
            {
                return null;
            }
            Event el = new Event(GetEvent(prot));
            if (!turn_pairs.ContainsKey(entity))
            {
                turn_pairs.Add(entity, new Turns(entity, el));
            } else
            {
                turn_pairs[entity].Push(el, turn);
            }
            return el;
        }

        public Event DoEvent(Entity e, Event prot)
        {
            Event el = new Event(prot);
            el.AddUpdates(e);
            el.Invoke(e);
            return el;
        }

        public Event DoEvent(Entity e, string prot)
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

