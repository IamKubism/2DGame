using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace HighKings
{
    public class Movers : IUpdater, ISystemAdder
    {
        public static Movers instance;

        ComponentSubscriber<Position> positions;

        public Dictionary<Entity, Position> entity_positions;

        /// <summary>
        /// The progress of movement
        /// </summary>
        public Dictionary<Entity, ItemVector<Position, FloatMinMax>> mover_progress;

        /// <summary>
        /// The paths that the movers will make to get to their destinations
        /// </summary>
        public Dictionary<Entity, Path_Astar> paths;

        public List<Entity> to_remove;

        public Movers()
        {
            paths = new Dictionary<Entity, Path_Astar>();
            mover_progress = new Dictionary<Entity, ItemVector<Position, FloatMinMax>>();
            to_remove = new List<Entity>();
            entity_positions = new Dictionary<Entity, Position>();
            positions = MainGame.instance.GetSubscriberSystem<Position>("Position");
            if(instance == null)
            {
                instance = this;
            }
            PrototypeLoader.instance.AddSystemLoc("movers", this);
        }

        public void Update(float dt)
        {
            Dictionary<Entity, object[]> disp_vals = new Dictionary<Entity, object[]>();
            Dictionary<Entity, object[]> tile_change = new Dictionary<Entity, object[]>();

            List<ItemVector<Position, Position, float>> to_update = new List<ItemVector<Position, Position, float>>();

            foreach(KeyValuePair<Entity, ItemVector<Position,FloatMinMax>> epf in mover_progress)
            {
                epf.Value.b += dt;

                if (epf.Value.b.IsOverMax())
                {
                    tile_change.Add(epf.Key, new object[1] { epf.Value.a });

                    //TODO: Make sure that the entity has some movement calculator if its assigned movement
                    MoveHander(epf.Key, MovementCalculator.test_calculator);

                } else
                {
                    to_update.Add(new ItemVector<Position, Position, float>(epf.Key.GetComponent<Position>("Position"), epf.Value.a, epf.Value.b.curr));
                    disp_vals.Add(epf.Key, new object[2] { epf.Value.a, epf.Value.b.curr });
                }
            }

            positions.UpdateComponents(disp_vals, DisplaceVector);
            positions.UpdateComponents(tile_change, SetTilePosition);

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

        public void DisplaceVector(Position p1, object[] updaters)
        {
            Position p2 = (Position)updaters[0];
            float d = (float)updaters[1];
            p1.SetDispPos((1 - d) * p1.t_r + d * p2.t_r);
        }

        public void SetTilePosition(Position p1, object[] updaters)
        {
            p1.UpdateToNewPoint((Position)updaters[0]);
        }

        public void DisplaceVectors(List<ItemVector<Position, Position, float>> to_update)
        {
            foreach (ItemVector<Position, Position, float> vec in to_update)
            {
                vec.a.SetDispPos((1 - vec.c) * vec.a.t_r + vec.c * vec.b.t_r);
            }
        }

        public void SetTilePositions(List<ItemVector<Position, Position>> vecs)
        {
            foreach (ItemVector<Position, Position> pp in vecs)
            {
                pp.a.UpdateToNewPoint(pp.b);
            }
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

            if (paths[e].Length() == 0)
            {
                paths.Remove(e);
            }
        }

        public void MoverPathMaker(Position[] end_area, Entity entity, IBehavior move_behavior)
        {
            //This in general should not happen but I am just going to do this for testing purposes
            IBehavior temp = move_behavior;
            if(temp == null)
            {
                Debug.Log("Set to default behavior");
                temp = MovementCalculator.test_calculator;
            }

            List<Entity> end = World.instance.GetTileArea(end_area);
            Path_Astar path = new Path_Astar(Path_TileGraph.movement_graph, entity, end, temp);
            if(path.Length() == 0)
            {
                return;
            }
            Entity next_cell = path.DeQueue();
            FloatMinMax prog = new FloatMinMax(0f, temp.CalculateOnEntity(next_cell));

            if(prog.max <= 0)
            {
                return;
            }

            mover_progress.Add(entity, new ItemVector<Position, FloatMinMax>(next_cell.GetComponent<Position>("Position"), prog));
            if (path.Length() > 0)
                paths.Add(entity, path);
            return;
        }

        public void MoverPathMaker(Position end_area, Entity entity, IBehavior move_behavior = default)
        {
            //This in general should not happen but I am just going to do this for testing purposes
            IBehavior temp = move_behavior;
            if(temp == default)
            {
                temp = MovementCalculator.test_calculator;
            }

            List<Entity> end = new List<Entity>{ World.instance.GetTileFromCoords(end_area.x,end_area.y,end_area.z) };
            Position start_p = entity.GetComponent<Position>("Position");
            Path_Astar path = new Path_Astar(Path_TileGraph.movement_graph, World.instance.GetTileFromCoords(start_p.x, start_p.y, start_p.z), end, temp);
            if(path.Length() == 0)
            {
                return;
            }

            Entity next_cell = path.DeQueue();
            FloatMinMax prog = new FloatMinMax(0f, temp.CalculateOnEntity(next_cell));
            if(prog.max <= 0)
            {
                return;
            }

            mover_progress.Add(entity, new ItemVector<Position, FloatMinMax>(next_cell.GetComponent<Position>("Position"), prog));
            if (path.Length() > 0)
                paths.Add(entity, path);
            return;
        }

        public void AddEntities(List<Entity> entities)
        {
            foreach(Entity e in entities)
            {
                if (entity_positions.ContainsKey(e) == false)
                {
                    entity_positions.Add(e, e.GetComponent<Position>("Position"));
                }
            }
        }

        public void OnAddedEntities(List<Entity> entities)
        {
            throw new NotImplementedException();
        }

        public string SysName()
        {
            return "Movers";
        }
    }
}