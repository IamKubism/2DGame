using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class Positions : ISystemAdder
    {
        public static Positions instance;
        Dictionary<Entity, Position> entity_positions;
        public Dictionary<string, Position> all_positions;
        Dictionary<string, Action> registered_entity_actions;

        Dictionary<Entity, Action<Entity>> individual_update_actions;

        Action<List<Entity>> on_added_entities;
        Action<List<Entity>> on_changed_entities;

        public Positions()
        {
            if(instance == null)
            {
                instance = this;   
            } else
            {
                Debug.LogError("Positions already created");
            }
            all_positions = new Dictionary<string, Position>();
            registered_entity_actions = new Dictionary<string, Action>();
            PrototypeLoader.instance.AddSystemLoc("entity_positions", this);
            entity_positions = new Dictionary<Entity, Position>();
            individual_update_actions = new Dictionary<Entity, Action<Entity>>();
        }

        public void SetTilePositions(List<ItemVector<Position, Position>> vecs)
        {
            foreach (ItemVector<Position, Position> pp in vecs)
            {
                pp.a.UpdateToNewPoint(pp.b);
            }
        }

        public void SetVectorDisplacement(Position p1, Position p2, float d)
        {
            p1.SetDispPos((1 - d) * p1.t_r + d * p2.t_r);
        }

        public void DisplaceVectors(List<ItemVector<Position, Position, float>> to_update)
        {
            foreach (ItemVector<Position, Position, float> vec in to_update)
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
            }
            else
            {
                individual_update_actions.Add(to_update, to_register);
            }
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

        //System add implementation

        public void AddEntities(List<Entity> entities)
        {
            foreach (Entity e in entities)
            {
                entity_positions.Add(e, e.GetComponent<Position>("Position"));
            }
        }

        public void OnAddedEntities(List<Entity> entities)
        {
            throw new NotImplementedException();
        }
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