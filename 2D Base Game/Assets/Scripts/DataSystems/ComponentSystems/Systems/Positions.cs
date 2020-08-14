using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighKings;

public class Positions : ISystemAdder
{
    public MapCells map_cells;
    Dictionary<Entity, Position> entity_positions;
    public Dictionary<string, Position> all_positions;
    Dictionary<string, Action> registered_entity_actions;

    Dictionary<Entity, Action<Entity>> individual_update_actions;

    List<string> rendered_entity_ids;

    Action<string> on_tile_pos_created;
    Action<string> on_tile_pos_changed;
    Action<string> on_disp_changed;
    Action<List<Entity>> on_added_entities;
    Action<List<Entity>> on_changed_entities;


    public Positions(string system_id = "entity_positions")
    {
        all_positions = new Dictionary<string, Position>();
        rendered_entity_ids = new List<string>();
        registered_entity_actions = new Dictionary<string, Action>();
        PrototypeLoader.instance.AddSystemLoc(system_id, this);
        entity_positions = new Dictionary<Entity, Position>();
        individual_update_actions = new Dictionary<Entity, Action<Entity>>();
    }

    /// <summary>
    /// Give an entity a position in tile space (automatically sets the vector position)
    /// </summary>
    /// <param name="entity_id"></param>
    /// <param name="position"></param>
    public void SetTilePosition(string entity_id, Position position)
    {
        if (all_positions.ContainsKey(entity_id))
        {
            all_positions[entity_id] = position;
            if (on_tile_pos_changed != null)
            {
                registered_entity_actions[entity_id]();
                on_tile_pos_changed(entity_id);
            }
        }
        else
        {
            all_positions.Add(entity_id, position);
            //Debug.Log($"Added {entity_id} at {position.ToString()}");
            registered_entity_actions.Add(entity_id, null);
            if (on_tile_pos_created != null)
            {
                on_tile_pos_created(entity_id);
            }
        }
    }

    public void SetTilePositions(List<ItemVector<Position,Position>> vecs)
    {
        foreach(ItemVector<Position, Position> pp in vecs)
        {
            pp.a.UpdateToNewPoint(pp.b);
        }
    }

    /// <summary>
    /// Transforms the current vector position to (1-p)*v_1 + p*v_2 (Where v_1, v_2 are tile positions)
    /// </summary>
    /// <param name="entity_id"></param>
    /// <param name="v_2"></param>
    /// <param name="p"></param>
    public void SetVectorDisplacement(string entity_id, Position v_2, float p)
    {
        //all_positions[entity_id] |= ((1 - p) * all_positions[entity_id] + p * v_2);
        if (on_disp_changed != null)
        {
            registered_entity_actions[entity_id]();
            on_disp_changed(entity_id);
        }
    }

    public void SetVectorDisplacement(Position p1, Position p2, float d)
    {
        p1.SetDispPos((1 - d) * p1.t_r + d * p2.t_r);
    }

    public void DisplaceVectors(List<ItemVector<Position,Position,float>> to_update)
    {
        foreach (ItemVector<Position,Position,float> vec in to_update)
        {
            vec.a.SetDispPos((1 - vec.c) * vec.a.t_r + vec.c * vec.b.t_r);
        }
    }

    public Vector3 DisplacedVector(string entity_id)
    {
        return all_positions[entity_id].disp_pos;
    }

    public int DistanceBetweenTilePositions(string entity_1, string entity_2)
    {
        return Position.SqrDist(all_positions[entity_1], all_positions[entity_2]);
    }

    /*
     * Registers
     */

    public void RegisterIndividualUpdateAction(Entity to_update, Action<Entity> to_register)
    {
        if (individual_update_actions.ContainsKey(to_update))
        {
            individual_update_actions[to_update] += to_register;
        } else
        {
            individual_update_actions.Add(to_update, to_register);
        }
    }

    public void RegisterOnDispChanged(Action<Positions, string> to_register)
    {
        on_disp_changed += (entity_id) => { to_register(this, entity_id); };
    }
    public void RegisterOnPosChanged(Action<Positions, string> to_register)
    {
        on_disp_changed += (entity_id) => { to_register(this, entity_id); };

    }
    public void RegisterOnPosCreated(Action<Positions, string> to_register)
    {
        on_disp_changed += (entity_id) => { to_register(this, entity_id); };
    }

    public void RegisterEntityChangeCB(string entity_id, Action<Positions, string> to_register)
    {
        registered_entity_actions[entity_id] += () => { to_register(this, entity_id); };
    }

    public void RegisterOnAddedEntities(Action<Positions, List<Entity>> action)
    {
        on_added_entities += (entities) => { action(this, entities); };
    }

    public void RegisterOnChangedEntities(Action<Positions, List<Entity>> action)
    {
        on_changed_entities += (entities) => { action(this, entities); };
    }


    /*
     * UnRegisters
     */

    public void UnRegisterOnDispChanged(Action<Positions, string> to_register)
    {
        on_disp_changed -= (entity_id) => { to_register(this, entity_id); };
    }
    public void UnRegisterOnPosChanged(Action<Positions, string> to_register)
    {
        on_disp_changed -= (entity_id) => { to_register(this, entity_id); };

    }
    public void UnRegisterOnPosCreated(Action<Positions, string> to_register)
    {
        on_disp_changed -= (entity_id) => { to_register(this, entity_id); };
    }


    //System add implementation

    public void AddEntities(List<Entity> entities)
    {
        foreach(Entity e in entities)
        {
            entity_positions.Add(e, e.GetComponent<Position>("Position"));
        }
    }

    public void OnAddedEntities(List<Entity> entities)
    {
        throw new NotImplementedException();
    }
}

//public void SetTilePosition(string entity_id, int x, int y, int z)
//{
//    if (all_positions.ContainsKey(entity_id))
//    {
//        all_positions[entity_id] = new Position(entity_id,x,y,z);
//        if (on_tile_pos_changed != null)
//        {
//            registered_entity_actions[entity_id]();
//            on_tile_pos_changed(entity_id);
//        }
//    }
//    else
//    {
//        all_positions.Add(entity_id, new Position(entity_id,x,y,z));
//        Debug.Log("Added Position for " + entity_id);
//        registered_entity_actions.Add(entity_id, null);
//        if (on_tile_pos_created != null)
//        {
//            on_tile_pos_created(entity_id);
//        }
//    }
//}