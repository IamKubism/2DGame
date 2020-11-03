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
        Dictionary<string, Event> events;
        SimplePriorityQueue<Event> queue;

        public EventQueue()
        {
            queue = new SimplePriorityQueue<Event>();
            events = new Dictionary<string, Event>();
        }

        public EventQueue(JProperty prop)
        {
            queue = new SimplePriorityQueue<Event>();
            events = new Dictionary<string, Event>();
            List<JToken> toks = prop.Value["listeners"].ToList();
            foreach (JProperty p in toks)
            {
                RegisterEvent(new Event(p));
            }
        }

        public EventQueue(Event listener)
        {
            queue = new SimplePriorityQueue<Event>();
            events = new Dictionary<string, Event>();
            events.Add(listener.id, listener);
            queue.Enqueue(listener, listener.priority);
        }

        public EventQueue(EventQueue event_queue)
        {
            queue = new SimplePriorityQueue<Event>();
            events = new Dictionary<string, Event>();
            RegisterQueue(event_queue);
        }

        public void Invoke(Entity target)
        {
            while(queue.Count > 0)
            {
                queue.Dequeue().Invoke(target);
            }
            events = new Dictionary<string, Event>();
        }

        public void RegisterEvent(Event listener, string id = "")
        {
            if (events.ContainsKey(id == "" ? listener.id : id) == false)
            {
                queue.Enqueue(listener, listener.priority);
                events.Add(id != "" ? id : listener.id, listener);
            }
        }

        public void RegisterQueue(EventQueue source_queue)
        {
            foreach(KeyValuePair<string,Event> es in source_queue.events)
            {
                RegisterEvent(es.Value, es.Key);
            }
        }

    }
}