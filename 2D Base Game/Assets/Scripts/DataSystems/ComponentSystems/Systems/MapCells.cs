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
        ComponentSubscriberSystem positions;
        Action<List<Entity>> pos_on_add_action;

        public MapCells()
        {
            world = World.instance;
            positions = MainGame.instance.GetSubscriberSystem<Position>();
            pos_on_add_action += (entities) =>
            {
                AddOccupants(entities);
                positions.SubscribeAfterAction<Position>(entities, AddOccupant, "CellAddOccupants");
                positions.SubscribeBeforeAction<Position>(entities, RemoveOccupant, "CellRemoveOccupants");
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
                Position p = e.GetComponent<Position>();
                Cell cell = world.GetTileFromCoords(p.p).GetComponent<Cell>();
                cell.AddOccupant(e);
            }
        }

        public void AddOccupant(Entity e, Position p)
        {
            Cell cell = world.GetTileFromCoords(p.p).GetComponent<Cell>();
            cell.AddOccupant(e);
        }

        public void RemoveOccupant(Entity entity, Position p)
        {
            Cell cell = world.GetTileFromCoords(p.p).GetComponent<Cell>();
            cell.RemoveOccupant(entity);
        }
    }
}
