using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Cell : IBaseComponent
    {
        [JsonProperty]
        public List<Entity> occupants { get; protected set; }

        public Cell(Entity entity)
        {
            occupants = new List<Entity>(8)
            {
                entity
            };
        }

        public Cell()
        {
            occupants = new List<Entity>(8);
        }

        public Cell(Cell c)
        {
            occupants = new List<Entity>(8);
        }

        public string ComponentType()
        {
            return "Cell";
        }

        public bool computable()
        {
            return false;
        }

        public void AddOccupant(Entity e)
        {
            if (occupants.Contains(e))
            {
                return;
            } 
            occupants.Add(e);
        }

        public void RemoveOccupant(Entity e)
        {
            occupants.Remove(e);
        }

        public List<Entity> GetOccupants()
        {
            return occupants;
        }

        public void OnUpdateState()
        {
            throw new System.NotImplementedException();
        }

        public void RegisterUpdateAction(Action<IBaseComponent> update)
        {

        }
    }
}

