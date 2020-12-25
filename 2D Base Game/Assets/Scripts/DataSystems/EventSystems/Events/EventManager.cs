using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Psingine
{
    public enum EventTags { DoPhysical }

    public class EventManager : ITriggeredUpdater, IUpdater
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
        public Dictionary<string, Dictionary<string, Event>> events_by_tag;
        HashSet<Entity> continuous_updaters;
        List<IEventObserver> event_observers;
        Event pass_time;
        public Dictionary<string, HashSet<IEventObserver>> observers_by_tag;

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
            continuous_updaters = new HashSet<Entity>();
            events_by_tag = new Dictionary<string, Dictionary<string, Event>>();
            event_prototypes.Add("NullEvent", new Event());
            event_observers = new List<IEventObserver>();
            observers_by_tag = new Dictionary<string, HashSet<IEventObserver>>();
        }

        public void Start()
        {
            pass_time = event_prototypes["PassTime"];
        }

        public void Update()
        {
            List<Turns> turns = (List<Turns>)turn_pairs.Values.AsEnumerable();
            for(int i = turns.Count; i > 0; i -= 1)
            {
                turns[i - 1].PopAndCall();
            }
        }

        public void InvokeEvent(Event e)
        {
            foreach(string s in e.tags)
            {
                foreach(IEventObserver observer in observers_by_tag[s])
                {
                    observer.Observe(e);
                }
            }
            e.Invoke();
            //I should actually probably be super careful about this, but its possible that an event might trigger another one so I would like to maybe do this
            //foreach(Event eve in e.Invoke())
            //{
            //    InvokeEvent(eve);
            //}
        }

        public void CallEvent(string event_id, Entity invoker)
        {
            Event inst = new Event(GetPrototype(event_id));
            inst.Invoke(invoker);
        }

        public void CallGlobalEvent(string event_id, Event prior)
        {
            Event inst = new Event(prior, event_id);
            foreach (IEventObserver observer in event_observers)
            {
                observer.Observe(inst);
            }
            inst.Invoke(prior.GetParamValue<Entity>(EventParams.invoking_entity));
        }

        public void Update(float dt)
        {
            //IDK if this stuff is necessary
            //HashSet<Entity> to_remove = new HashSet<Entity>();
            //foreach(Entity e in continuous_updaters)
            //{
            //    Event pass = new Event(pass_time);
            //    pass.SetParamValue("dt", dt, (f1,f2) => { return f2; });
            //    pass.SetParamValue("continue_update", false, (f1, f2) => { return f1; });

            //    //Need to find a good way to tell it that the thing isn't updating anymore so that we don't basically have a memory leak
            //    pass.Invoke(e);
            //    if(!pass.GetParamValue<bool>("continue_update"))
            //        to_remove.Add(e);
            //}
            //foreach(Entity e in to_remove)
            //{
            //    RemoveFromContinuousUpdate(e);
            //}
        }

        public void AddObserver(string[] tags, IEventObserver observer)
        {
            foreach(string tag in tags)
            {
                if(observers_by_tag.TryGetValue(tag, out HashSet<IEventObserver> obs))
                {
                    obs.Add(observer);
                } else
                {
                    observers_by_tag.Add(tag, new HashSet<IEventObserver> { observer });
                }
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

        public Event GetEvent(string id, bool allow_new_prototype = false)
        {
            if (!event_prototypes.TryGetValue(id, out Event e))
            {
                Debug.LogWarning($"Could not find event {id}");
                if (allow_new_prototype)
                {
                    //TODO: Make a new event
                }
            }
            return e;
        }

        public void AddEventPrototype(JProperty p)
        {
            event_prototypes.Add(p.Name, new Event(p));
            if (p.Value["external_tags"] != null)
            {
                List<JToken> ext_tags = p.Value["external_tags"].ToList();
                foreach(JToken tok in ext_tags)
                {
                    if(events_by_tag.TryGetValue(tok.Value<string>(), out Dictionary<string, Event> dict))
                    {
                        dict.Add(p.Name, event_prototypes[p.Name]);
                    } else
                    {
                        events_by_tag.Add(tok.Value<string>(), new Dictionary<string, Event> { { p.Name, event_prototypes[p.Name] } });
                    }
                }
            }
        }

        public Event GetPrototype(string name)
        {
            if (event_prototypes.TryGetValue(name, out Event val))
            {
                return val;
            }
            Debug.LogWarning($"Could not find event: {name}");
            return default;
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
            el.Alter(e);
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
            el.Alter(e);
            el.Invoke(e);
            return el;
        }

    }
}

