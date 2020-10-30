using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace HighKings
{
    public class Cell : IBaseComponent
    {
        public List<string> occupants;
        public SubscriberEvent subscriber { get; set; }

        public Cell()
        {
            occupants = new List<string>();
        }

        public Cell(JObject obj)
        {
            occupants = new List<string>();
            if(obj["occupants"] != null)
            {
                foreach (JToken tok in obj["occupants"].ToList())
                {
                    occupants.Add(tok.ToString());
                }
            }
        }

        public Cell(Cell c)
        {
            occupants = new List<string>(c.occupants);
        }

        public bool Trigger(Event e)
        {
            bool eval = true;
            foreach(Entity o in occupants)
            {
                e.AddUpdates(o);
            }
            return eval;
        }

        public void AddOccupant(Entity e, bool call_subscriber = false)
        {
            if (call_subscriber)
                subscriber.OperateBeforeOnComp();
            if (occupants.Contains(e))
            {
                return;
            }
            occupants.Add(e);
            if (call_subscriber)
                subscriber.OperateBeforeOnComp();
        }

        public void RemoveOccupant(Entity e, bool call_subscriber = false)
        {
            if (call_subscriber)
                subscriber.OperateBeforeOnComp();
            if (occupants.Contains(e))
            {
                occupants.Remove(e);
            }
            if (call_subscriber)
                subscriber.OperateBeforeOnComp();
        }

        public bool Contains(Entity e)
        {
            return occupants.Contains(e);
        }
    }
}

