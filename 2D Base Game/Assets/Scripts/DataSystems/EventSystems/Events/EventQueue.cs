using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using Priority_Queue;

namespace HighKings
{
    public class EventQueue
    {
        SimplePriorityQueue<Event> queue;

        public EventQueue()
        {
            queue = new SimplePriorityQueue<Event>();
        }

        public EventQueue(JProperty prop)
        {
            queue = new SimplePriorityQueue<Event>();
            List<JToken> toks = prop.Value["listeners"].ToList();
            foreach (JProperty p in toks)
            {
                RegisterEvent(new Event(p));
            }
        }

        public EventQueue(Event listener)
        {
            queue = new SimplePriorityQueue<Event>();
            queue.Enqueue(listener, listener.priority);
        }

        public EventQueue(EventQueue event_queue)
        {
            queue = new SimplePriorityQueue<Event>();
            foreach(Event l in event_queue.queue)
            {
                queue.Enqueue(l, l.priority);
            }
        }

        public void Invoke(Entity target)
        {
            while(queue.Count > 0)
            {
                queue.Dequeue().Invoke(target);
            }
        }

        public void RegisterEvent(Event listener)
        {
            queue.Enqueue(listener, listener.priority);
        }

        public void RegisterQueue(EventQueue source_queue)
        {
            foreach(Event e in source_queue.queue)
            {
                queue.Enqueue(e, e.priority);
            }
        }

        /// <summary>
        /// Creates a unioned action (calls a unioned set of the listeners of a and b)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static EventQueue operator +(EventQueue a, EventQueue b)
        {
            EventQueue c = new EventQueue(a);
            foreach(Event l in c.queue)
            {
                c.RegisterEvent(l);
            }
            return c;
        }

    }
}