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
        readonly Entity this_cell;

        [JsonProperty]
        List<Entity> occupants;

        public Cell(Entity entity)
        {
            this_cell = entity;
            occupants = new List<Entity>(8);
        }

        public Cell()
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

        public int[] Index
        {
            get
            {
                return this_cell.GetComponent<Position>("Position").p;
            }
        }

        public void AddOccupant(Entity e)
        {
            if (occupants.Contains(e))
            {
                Debug.LogError($"Occupant {e.entity_string_id} is already in the cell {this_cell.entity_string_id}");
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
            return new List<Entity>(occupants)
            {
                this_cell
            };
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

