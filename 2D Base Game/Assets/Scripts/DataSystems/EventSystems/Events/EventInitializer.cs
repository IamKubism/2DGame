using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    /// <summary>
    /// Class that gets called on events being created (for params/ updates that entities/components wouldn't know about)
    /// </summary>
    public class EventInitializer
    {
        public static EventInitializer instance;
        Dictionary<string, Event> default_events;

        public EventInitializer()
        {
            if (instance == null)
            {
                instance = this;
            } else
            {
                Debug.LogWarning($"Tried to create event initializer twice");
            }
            default_events = new Dictionary<string, Event>();
            Event add_to_time_queue_event = new Event("AddToTimeQueue", "AddToTimeQueue", 100);
            add_to_time_queue_event.AddUpdate(AddToTimeQueue, 100);
            default_events.Add("AddToTimeQueue", add_to_time_queue_event);
        }

        public void RegisterUpdates(List<Event> events)
        {
            foreach(Event e in events)
            {
                foreach(string tag in e.tags)
                {
                    if (default_events.TryGetValue(tag, out Event defe))
                    {
                        e.AppendEvent(defe);
                    }
                }
            }
        }

        public static void AddToTimeQueue(Event e)
        {
            EventManager.instance.AddToContinuousUpdate(e.GetParamValue<Entity>("invoking_entity"));
        }
    }
}

/*  Default Events are ones that are more based on the game overall than on specific entites
 *      Example: If an event is "time based" the initializer will automatically copy the entity to the updaters that are called every frame (for optimization reasons)
 *      
 */