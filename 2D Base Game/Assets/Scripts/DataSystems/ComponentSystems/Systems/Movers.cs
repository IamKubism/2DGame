//////////////////////////////////////////////
////// Mover Class
////// Last Updated: Version 0.0.0 11/06/2020
//////////////////////////////////////////////


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Psingine
{
    public enum MovementParams
    {
        curr_tile,
        next_tile,
        total_move_progress,
        displaced_position,
        terrain_cost,
        move_cost,
        move_progress,
        speed,
        movement_cost
    }

    public enum MovementEvents
    {
        SetMoveDestination,
        ComputeMovementProgress,
        SetTile,
        SetMovementData,
        CalculateMovementCost,
        StopMovement,
        SetDisplacedPosition
    }

    public class Movers : IUpdater, ISystemAdder
    {
        public static Movers instance;
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
            if(instance == null)
            {
                instance = this;
            }
            PrototypeLoader.instance.AddSystemLoc("movers", this);
            EventManager.instance.GetPrototype(MovementEvents.SetMoveDestination.ToString()).AddUpdate(MakePath, 100);
            EventManager.instance.GetPrototype(MovementEvents.StopMovement.ToString()).AddUpdate(RemoveFromMoverProgress, 10);
        }


        /// <summary>
        /// TODO: Trying to make this event based / not incredibly complicated
        /// </summary>
        /// <param name="dt"></param>
        public void Update(float dt)
        {
            List<ItemVector<Position, Position, float>> to_update = new List<ItemVector<Position, Position, float>>();

            foreach (KeyValuePair<Entity, ItemVector<Position, FloatMinMax>> epf in mover_progress)
            {
                Event prog_mov = Event.NewEvent(MovementEvents.ComputeMovementProgress);
                prog_mov.AddUpdate((e) => { e.SetParamValue(MovementParams.move_progress, 0f, (f1, f2) => { return f1; }); }, 0);
                prog_mov.AddUpdate((e) => { e.SetParamValue(MovementParams.move_progress, dt, (f1, f2) => { return f1 * f2; }); }, 100);

                epf.Value.b += prog_mov.Invoke<float>(epf.Key, MovementParams.move_progress);

                if (epf.Value.b.IsOverMax())
                {
                    Event set_next_tile = Event.NewEvent(MovementEvents.SetTile);
                    set_next_tile.AddUpdate((e) => { e.SetParamValue(MovementParams.curr_tile, epf.Value.a.ToTile(), (p1, p2) => { return p2; }); }, 10);
                    set_next_tile.Invoke(epf.Key);

                    if (paths.ContainsKey(epf.Key))
                    {
                        //Basically the new move hander
                        Event calc_move = Event.NewEvent(MovementEvents.SetMovementData);
                        Entity t = paths[epf.Key].DeQueue();
                        calc_move.Invoke(t);
                        calc_move = Event.NewEvent(calc_move, MovementEvents.CalculateMovementCost);

                        float next_cost = calc_move.Invoke<float>(epf.Key, MovementParams.movement_cost);
                        if (next_cost <= 0)
                        {
                            Path_Astar new_path = new Path_Astar(World.instance.graph, epf.Key, epf.Value.a.ToTile(), paths[epf.Key].end_cells);
                            if (new_path.Length() == 0)
                            {
                                to_remove.Add(epf.Key);
                                continue;
                            }
                            else
                            {
                                paths[epf.Key] = new_path;
                                t = new_path.DeQueue();
                                calc_move = Event.NewEvent(MovementEvents.SetMovementData);
                                calc_move.Invoke(t);
                                calc_move = Event.NewEvent(calc_move, MovementEvents.CalculateMovementCost);
                                next_cost = calc_move.Invoke<float>(epf.Key, MovementParams.movement_cost);
                            }
                        }
                        epf.Value.a = t.GetComponent<Position>();
                        epf.Value.b.Reset((epf.Value.a.t_r - epf.Key.GetComponent<Position>().t_r).magnitude * next_cost);
                        if (paths[epf.Key].Length() == 0)
                        {
                            paths.Remove(epf.Key);
                        }
                    }
                    else
                    {
                        to_remove.Add(epf.Key);
                    }
                }
                else
                {
                    Event ev = Event.NewEvent(MovementEvents.SetDisplacedPosition);
                    ev.SetParamValue(MovementParams.next_tile, epf.Value.a.ToTile(), (a, b) => { return b; });
                    ev.SetParamValue(MovementParams.total_move_progress, epf.Value.b.NormalizedByMax(), (f1, f2) => { return f2; });
                    ev.AddUpdate(EventDisplaceVector, 40);
                    ev.Invoke(epf.Key);
                }
            }

            for (int i = to_remove.Count; i > 0; i -= 1)
            {
                Event remove = Event.NewEvent(MovementEvents.StopMovement);
                remove.Invoke(to_remove[i - 1]);
                to_remove.RemoveAt(i - 1);
            }
        }

        public void EventDisplaceVector(Event e)
        {
            Entity curr_tile = e.GetParamValue<Entity>(MovementParams.curr_tile);
            Entity next_tile = e.GetParamValue<Entity>(MovementParams.next_tile);
            float total_move_progress = e.GetParamValue<float>(MovementParams.total_move_progress);
            e.SetParamValue(MovementParams.displaced_position, DisplacedVector(curr_tile.GetComponent<Position>(), total_move_progress , next_tile.GetComponent<Position>()), (t1,t2) => { return t2; });
        }

        public Vector3 DisplacedVector(Position p1, float dr, Position direction)
        {
            return (1 - dr) * p1.t_r + dr * direction.t_r;
        }

        public bool TryGetTarget(Entity e, out Entity target)
        {
            if(paths.TryGetValue(e, out Path_Astar p))
            {
                target = p.Last();
                return true;
            } else if (mover_progress.TryGetValue(e, out ItemVector<Position,FloatMinMax> pf))
            {
                target = pf.a.ToTile();
                return true;
            } else
            {
                target = e;
                return false;
            }
        }

        public void MakePath(Event e)
        {
            //Debug.Log("Making path");
            Entity mover = e.GetParamValue<Entity>(EventParams.invoking_entity);
            HashSet<Entity> targs = e.GetParamValue<HashSet<Entity>>(EventParams.targets);

            //Might have a better way of doing this
            List<Entity> dest = new List<Entity>();
            foreach(Entity ent in targs)
            {
                if (ent.GetComponent<EntityType>().type_name == "tile")
                {
                    dest.Add(ent);
                }
            }

            Path_Astar path = new Path_Astar(World.instance.graph, mover, mover.GetComponent<Position>().ToTile(), dest);
            if(path.Length() == 0)
            {
                if (mover_progress.ContainsKey(mover))
                {
                    mover_progress.Remove(mover);
                }
            } else
            {
                if (mover_progress.TryGetValue(mover, out ItemVector<Position, FloatMinMax> val))
                {
                } else
                {
                    mover_progress.Add(mover, new ItemVector<Position, FloatMinMax>(path.DeQueue().GetComponent<Position>(), new FloatMinMax()));
                }
                //Debug.Log("Added mover");
            }
            if(path.Length() == 0)
            {
                if (paths.ContainsKey(mover))
                {
                    paths.Remove(mover);
                }
            } else
            {
                if (paths.ContainsKey(mover))
                {
                    paths[mover] = path;
                }
                else
                {
                    paths.Add(mover, path);
                }
                //Debug.Log("Added path");
            }
        }

        public void DisplaceVector(Position p1, params object[] updaters)
        {
            Position p2 = (Position)updaters[0];
            float d = (float)updaters[1];
            p1.SetDispPos((1 - d) * p1.t_r + d * p2.t_r);
        }

        public void SetTilePosition(Position p1, params object[] updaters)
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

        void AddToMoverProgress(Entity e, ItemVector<Position, FloatMinMax> pf)
        {
            mover_progress.Add(e, pf);
        }

        void RemoveFromMoverProgress(Entity e)
        {
            if (paths.ContainsKey(e))
            {
                paths.Remove(e);
            }
            mover_progress.Remove(e);
            //Debug.Log("removed " + e.ToString());
        }

        void RemoveFromMoverProgress(Event e)
        {
            Entity invoker = e.GetParamValue<Entity>(EventParams.invoking_entity);
            if (paths.ContainsKey(invoker))
            {
                paths.Remove(invoker);
            }
            mover_progress.Remove(invoker);
            //Debug.Log("removed " + e.ToString());
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

        public void AddEntity(Entity e)
        {
            if (entity_positions.ContainsKey(e) == false)
            {
                entity_positions.Add(e, e.GetComponent<Position>("Position"));
            }
        }

        public string SysCompName()
        {
            return "Movers";
        }
    }
}

//public void MoverPathMaker(Position[] end_area, Entity entity, IBehavior move_behavior)
//{
//    Debug.Log("Making path");

//    //This in general should not happen but I am just going to do this for testing purposes
//    IBehavior temp = move_behavior;
//    if(temp == null)
//    {
//        Debug.Log("Set to default behavior");
//        temp = MovementCalculator.test_calculator;
//    }

//    List<Entity> end = World.instance.GetTileArea(end_area);
//    Path_Astar path = new Path_Astar(Path_TileGraph.movement_graph, entity, end, temp);

//    if (path.Length() == 0)
//    {
//        return;
//    }

//    if (paths.ContainsKey(entity) == false)
//    {
//        if (mover_progress.ContainsKey(entity))
//        {
//            paths.Add(entity, path);
//            return;
//        }
//        Entity next_cell = path.DeQueue();
//        FloatMinMax prog = new FloatMinMax(0f, temp.CalculateOnEntity(next_cell));
//        if (prog.max <= 0)
//        {
//            return;
//        }
//        AddToMoverProgress(entity, new ItemVector<Position, FloatMinMax>(next_cell.GetComponent<Position>("Position"), prog));
//        if (path.Length() > 0)
//            paths.Add(entity, path);
//    } else
//    {
//        paths[entity] = path;
//    }
//}

//public void MoverPathMaker(List<Entity> end_area, Entity entity, IBehavior move_behavior = default)
//{
//    Debug.Log("Making path");

//    //This in general should not happen but I am just going to do this for testing purposes
//    IBehavior temp = move_behavior;
//    if(temp == null)
//    {
//        Debug.Log("Set to default behavior");
//        temp = MovementCalculator.test_calculator;
//    }
//    Path_Astar path = new Path_Astar(Path_TileGraph.movement_graph, entity, end_area, temp);
//    if (path.Length() == 0)
//    {
//        return;
//    }

//    if (paths.ContainsKey(entity) == false)
//    {
//        if (mover_progress.ContainsKey(entity))
//        {
//            paths.Add(entity, path);
//            return;
//        }
//        Entity next_cell = path.DeQueue();
//        FloatMinMax prog = new FloatMinMax(0f, temp.CalculateOnEntity(next_cell));
//        if (prog.max <= 0)
//        {
//            return;
//        }
//        AddToMoverProgress(entity, new ItemVector<Position, FloatMinMax>(next_cell.GetComponent<Position>("Position"), prog));
//        if (path.Length() > 0)
//            paths.Add(entity, path);
//    } else
//    {
//        paths[entity] = path;
//    }
//}

//public void MoverPathMaker(Position end_area, Entity entity, IBehavior move_behavior = default)
//{
//    //if (!entity.HasComponent("PhysicalActive"))
//    //{
//    //    Debug.LogError($"{entity.entity_string_id} does not have a PhysicalActive flag");
//    //    return;
//    //}
//    Debug.Log("Making path");


//    IBehavior temp = move_behavior;
//    if(temp == default)
//    {
//        temp = MovementCalculator.test_calculator;
//    }

//    List<Entity> end = new List<Entity>{ World.instance.GetTileFromCoords(end_area.x,end_area.y,end_area.z) };
//    Position start_p = entity.GetComponent<Position>("Position");
//    Path_Astar path = new Path_Astar(Path_TileGraph.movement_graph, World.instance.GetTileFromCoords(start_p.x, start_p.y, start_p.z), end, temp);

//    if (path.Length() == 0)
//    {
//        return;
//    }

//    if(paths.ContainsKey(entity) == false)
//    {
//        if (mover_progress.ContainsKey(entity))
//        {
//            paths.Add(entity, path);
//            return;
//        }
//        Entity next_cell = path.DeQueue();
//        FloatMinMax prog = new FloatMinMax(0f, temp.CalculateOnEntity(next_cell));
//        if (prog.max <= 0)
//        {
//            return;
//        }

//        AddToMoverProgress(entity, new ItemVector<Position, FloatMinMax>(next_cell.GetComponent<Position>("Position"), prog));
//        if (path.Length() > 0)
//            paths.Add(entity, path);
//        return;
//    } else
//    {
//        paths[entity] = path;
//    }
//}

//public static void MoverPathMaker(Entity source, Entity target)
//{
//    Debug.Log("Making path");
//    IBehavior temp = MovementCalculator.test_calculator;

//    Position end_p = target.GetComponent<Position>("Position");
//    List<Entity> end = new List<Entity> { World.instance.GetTileFromCoords(end_p.x, end_p.y, end_p.z) };
//    Position start_p = source.GetComponent<Position>("Position");
//    Path_Astar path = new Path_Astar(Path_TileGraph.movement_graph, World.instance.GetTileFromCoords(start_p.x, start_p.y, start_p.z), end, temp);

//    if (path.Length() == 0)
//    {
//        return;
//    }

//    if (instance.paths.ContainsKey(source) == false)
//    {
//        if (instance.mover_progress.ContainsKey(source))
//        {
//            instance.paths.Add(source, path);
//            return;
//        }
//        Entity next_cell = path.DeQueue();
//        FloatMinMax prog = new FloatMinMax(0f, temp.CalculateOnEntity(next_cell));
//        if (prog.max <= 0)
//        {
//            return;
//        }

//        instance.AddToMoverProgress(source, new ItemVector<Position, FloatMinMax>(next_cell.GetComponent<Position>("Position"), prog));
//        if (path.Length() > 0)
//            instance.paths.Add(source, path);
//        return;
//    }
//    else
//    {
//        instance.paths[source] = path;
//    }
//}

//public static void CancelMove(Entity source, Entity target)
//{
//    Debug.Log("Cancelling move path");

//    MoverPathMaker(source, World.instance.GetTileFromCoords(source.GetComponent<Position>("Position").p));
//}

//public void MoveHander(Entity e, IBehavior movement_behavior)
//{
//    if (paths.ContainsKey(e) == false)
//    {
//        to_remove.Add(e);
//        return;
//    }

//    //e.GetComponent<FlagComponent>("PhysicalActive").SetActive();
//    Entity next_cell = paths[e].DeQueue();
//    mover_progress[e].a = next_cell.GetComponent<Position>("Position");

//    if(movement_behavior.CalculateOnEntity(next_cell) <= 0)
//    {
//        mover_progress.Remove(e);
//        paths.Remove(e);
//        MoverPathMaker(new Position[1] { next_cell.GetComponent<Position>() }, e, movement_behavior);
//    } else
//        mover_progress[e].b.Reset(movement_behavior.CalculateOnEntity(next_cell));

//    if (paths[e].Length() == 0)
//    {
//        paths.Remove(e);
//    }
//}
//public void Update(float dt)
//{
//    Dictionary<Entity, object[]> disp_vals = new Dictionary<Entity, object[]>();
//    Dictionary<Entity, object[]> tile_change = new Dictionary<Entity, object[]>();
//    List<ItemVector<Position, Position, float>> to_update = new List<ItemVector<Position, Position, float>>();

//    foreach(KeyValuePair<Entity, ItemVector<Position,FloatMinMax>> epf in mover_progress)
//    {
//        epf.Value.b += dt;

//        if (epf.Value.b.IsOverMax())
//        {
//            tile_change.Add(epf.Key, new object[1] { epf.Value.a });

//            //TODO: Make sure that the entity has some movement calculator if its assigned movement
//            MoveHander(epf.Key, MovementCalculator.test_calculator);

//        } else
//        {
//            to_update.Add(new ItemVector<Position, Position, float>(epf.Key.GetComponent<Position>(), epf.Value.a, epf.Value.b.curr));
//            disp_vals.Add(epf.Key, new object[2] { epf.Value.a, epf.Value.b.curr });
//        }
//    }

//    positions.UpdateComponents<Position>(disp_vals, DisplaceVector);
//    positions.UpdateComponents<Position>(tile_change, SetTilePosition);

//    for (int i = to_remove.Count; i > 0; i -= 1)
//    {
//        RemoveFromMoverProgress(to_remove[i - 1]);
//        to_remove.RemoveAt(i - 1);
//    }
//}
