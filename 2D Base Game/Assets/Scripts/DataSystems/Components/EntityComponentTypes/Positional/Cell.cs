/////////////////////////////////////////////
/// Cell Component for keeping track of entities in a cell
/// Last Updated: Version 0.0.0 10/30/2020
/////////////////////////////////////////////


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Psingine
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
                //TODO //e.Alter(o);
            }
            return eval;
        }

        /// <summary>
        /// Add an occupant to a cell on the map
        /// </summary>
        /// <param name="e"></param>
        /// <param name="call_subscriber"></param>
        public void AddOccupant(string e, bool call_subscriber = false)
        {
            if (call_subscriber)
                subscriber.OperateBeforeOnComp();
            if (occupants.Contains(e) || ((Entity)e).HasComponent("Cell"))
            {
                return;
            }
            occupants.Add(e);
            if (call_subscriber)
                subscriber.OperateAfterOnComp();
        }

        /// <summary>
        /// Remove an occupant from a cell on the map
        /// </summary>
        /// <param name="e"></param>
        /// <param name="call_subscriber"></param>
        public void RemoveOccupant(string e, bool call_subscriber = false)
        {
            if (call_subscriber)
                subscriber.OperateBeforeOnComp();
            if (occupants.Contains(e) || ((Entity)e).HasComponent("Cell"))
            {
                occupants.Remove(e);
            }
            if (call_subscriber)
                subscriber.OperateAfterOnComp();
        }

        /// <summary>
        /// Really more of a debugging thing. 
        /// </summary>
        /// <param name="call_subscriber"></param>
        public void CheckOccupants(bool call_subscriber = false)
        {
            if (call_subscriber)
                subscriber.OperateBeforeOnComp();
            foreach (string occupant in occupants)
            {
                if (!Entity.Manager().CheckExistance(occupant))
                    RemoveOccupant(occupant);
            }
            if (call_subscriber)
                subscriber.OperateAfterOnComp();
        }

        public bool Contains(Entity e)
        {
            return occupants.Contains(e);
        }

        public void CopyData(IBaseComponent comp)
        {
            throw new System.NotImplementedException();
        }
    }
}

