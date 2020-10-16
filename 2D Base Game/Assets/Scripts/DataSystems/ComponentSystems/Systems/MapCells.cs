using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    /// <summary>
    /// Currently I am just trying to get things working for a 1 chunck world. This will store all of the occupancy type data for map tiles and mangage them
    /// </summary>
    public class MapCells
    {
        World world;
        ComponentSubscriberSystem<Position> positions;
        Action<List<Entity>> pos_on_add_action;

        public MapCells()
        {
            world = World.instance;
            positions = MainGame.instance.GetSubscriberSystem<Position>("Position");
            pos_on_add_action += (entities) =>
            {
                AddOccupants(entities);
                positions.SubscribeAfterAction(entities, AddOccupant, "CellAddOccupants");
                positions.SubscribeBeforeAction(entities, RemoveOccupant, "CellRemoveOccupants");
            };
            positions.RegisterOnAdded(pos_on_add_action);
        }

        /// <summary>
        /// For creation of entities with positions, to reset cell position instead use "SwapOccupants"
        /// </summary>
        /// <param name="entities"></param>
        public void AddOccupants(List<Entity> entities)
        {
            foreach(Entity e in entities)
            {
                Position p = e.GetComponent<Position>("Position");
                List<Entity> cell = world.tile_map.cells[p.x, p.y, p.z];
                if (cell.Contains(e))
                {
                    continue;
                } else
                {
                    cell.Add(e);
                }
            }
        }

        public void AddOccupant(Entity e, Position p)
        {
            List<Entity> cell = world.tile_map.cells[p.x, p.y, p.z];
            if (cell.Contains(e))
            {
                Debug.LogError("Tried to add an entity to a cell more than once");
            }
            else
            {
                cell.Add(e);
            }
        }

        public void RemoveOccupant(Entity entity, Position p)
        {
            List<Entity> cell = world.tile_map.cells[p.x, p.y, p.z];
            if (cell.Contains(entity) == false)
            {
                Debug.LogError("Tried to remove an entity from a cell it is not part of");
            }
            else
            {
                cell.Remove(entity);
            }
        }
    }
}
