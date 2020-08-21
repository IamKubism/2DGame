using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace HighKings
{
    public class Movers
    {
        Positions positions_current;
        Path_TileGraph graph;

        /// <summary>
        /// The progress of movement
        /// </summary>
        public Dictionary<Entity, ItemVector<Position, FloatMinMax>> mover_progress;

        /// <summary>
        /// The paths that the movers will make to get to their destinations
        /// </summary>
        public Dictionary<Entity, Path_Astar> paths;

        public List<Entity> to_remove;

        Action<string> on_created_mover;
        Action<string> on_movement_ended;
        Action<string> on_mover_changed;

        public Movers(Path_TileGraph graph, Positions positions_current)
        {
            this.graph = graph;
            paths = new Dictionary<Entity, Path_Astar>();
            mover_progress = new Dictionary<Entity, ItemVector<Position, FloatMinMax>>();
            to_remove = new List<Entity>();
            this.positions_current = positions_current;
        }

        public void UpdateMovement(float dt)
        {
            List<ItemVector<Position, Position, float>> to_update = new List<ItemVector<Position, Position, float>>();
            List<ItemVector<Position, Position>> tile_change = new List<ItemVector<Position, Position>>();

            foreach(KeyValuePair<Entity, ItemVector<Position,FloatMinMax>> epf in mover_progress)
            {
                epf.Value.b += dt;

                if (epf.Value.b.IsOverMax())
                {
                    tile_change.Add(new ItemVector<Position, Position>(epf.Key.GetComponent<Position>("Position"), epf.Value.a));

                    //TODO: Make sure that the entity has some movement calculator if its assigned movement
                    MoveHander(epf.Key, MovementCalculator.test_calculator);

                } else
                {
                    to_update.Add(new ItemVector<Position, Position, float>(epf.Key.GetComponent<Position>("Position"), epf.Value.a, epf.Value.b.curr));
                }
            }

            positions_current.SetTilePositions(tile_change);
            positions_current.DisplaceVectors(to_update);

            for (int i = to_remove.Count; i > 0; i -= 1)
            {
                mover_progress.Remove(to_remove[i - 1]);
                to_remove.RemoveAt(i - 1);
            }

            //for (int i = active_movers.Count; i > 0; i -= 1)
            //{
            //    string entity_id = active_movers[i - 1];
            //    FloatMinMax to_check = mover_progs[entity_id] + dt;
            //    if (to_check.IsOverMax())
            //    {
            //        positions_current.SetTilePosition(entity_id, mover_positions_next[entity_id]);
            //        if (movement_queue.ContainsKey(entity_id))
            //        {
            //            MoveHander(entity_id);
            //        }
            //        else
            //        {
            //            mover_positions_next.Remove(entity_id);
            //            mover_progs.Remove(entity_id);
            //            active_movers.Remove(entity_id);
            //            on_movement_ended(entity_id);
            //        }
            //    }
            //    else
            //    {
            //        if (on_mover_changed != null)
            //        {
            //            on_mover_changed(entity_id);
            //        }
            //        positions_current.SetVectorDisplacement
            //            (entity_id, mover_positions_next[entity_id], to_check.NormalizedByMax());
            //        mover_progs[entity_id] = to_check;
            //    }
            //}
        }

        public void MoveHander(Entity e, IBehavior movement_behavior)
        {
            if (paths.ContainsKey(e) == false)
            {
                to_remove.Add(e);
                return;
            }
            Entity next_cell = paths[e].DeQueue();
            mover_progress[e].a = next_cell.GetComponent<Position>("Position");
            if(movement_behavior.CalculateOnEntity(next_cell) <= 0)
            {
                mover_progress.Remove(e);
                paths.Remove(e);
                MoverPathMaker(new Position[1] { next_cell.GetComponent<Position>("Position") }, e, movement_behavior);
            } else
                mover_progress[e].b.Reset(movement_behavior.CalculateOnEntity(next_cell));
            if (paths[next_cell].Length() == 0)
            {
                paths.Remove(next_cell);
            }
        }
        
        public void MoverPathMaker(Position[] end_area, Entity entity, IBehavior move_behavior)
        {
            List<Entity> end = positions_current.map_cells.GetCellArea(end_area);
            Path_Astar path = new Path_Astar(graph, entity, end, move_behavior);
            if(path.Length() == 0)
            {
                return;
            }
            Entity next_cell = path.DeQueue();
            FloatMinMax prog = new FloatMinMax(0f, move_behavior.CalculateOnEntity(next_cell));
            if(prog.max <= 0)
            {
                return;
            }
            mover_progress.Add(entity, new ItemVector<Position, FloatMinMax>(next_cell.GetComponent<Position>("Position"), prog));
            if (path.Length() > 0)
                paths.Add(entity, path);
        }

        public void RegisterOnMoverCreated(Action<Movers, string> to_reg)
        {
            on_created_mover += (entity_id) => { to_reg(this, entity_id); };
        }

        public void RegisterOnMoverEnded(Action<Movers, string> to_reg)
        {
            on_movement_ended += (entity_id) => { to_reg(this, entity_id); };
        }

        public void RegisterOnMoverChanged(Action<Movers, string> to_reg)
        {
            on_mover_changed += (entity_id) => { to_reg(this, entity_id); };
        }
    }
}