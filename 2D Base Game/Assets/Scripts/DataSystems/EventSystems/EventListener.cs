using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public class EventListener
    {
        public string id;
        public int priority;
        Action<Entity> to_call;

        public EventListener()
        {
            id = "null";
            priority = 100;
        }

        public EventListener(JProperty p)
        {

        }

        public EventListener(EventListener el)
        {

        }

        public void Invoke(Entity e)
        {

        }


    }
}

