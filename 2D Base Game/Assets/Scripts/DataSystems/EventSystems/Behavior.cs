using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public abstract class Behavior
    {
        public int priority = 1000;
        public string id = "";

        public Behavior() { }

        public Behavior(string id, int priority)
        {
            this.id = id;
            this.priority = priority;
        }

        public Behavior(Behavior b)
        {
            id = b.id;
            priority = b.priority;
        }

        public abstract void Trigger(Event e);
    }
}

