using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace HighKings
{
    /// <summary>
    /// Class Designed exclusively to just keep track of entities
    /// </summary>
    public class EntityManager
    {
        public static int entity_num;

        /// <summary>
        /// For everything to reference, for ease of use
        /// </summary>
        public static EntityManager instance;

        /// <summary>
        /// This is used near entirely for creating nicer looking save files
        /// </summary>
        Dictionary<string, List<Entity>> entities;

        Dictionary<string, Entity> string_id_to_entity;
        List<Entity> entity_array;

        public EntityManager()
        {
            if(instance != null)
            {
                Debug.LogError("Entity manager already exists");
            } else
            {
                instance = this;
                entity_num = 0;
            }
            entities = new Dictionary<string, List<Entity>>();
            string_id_to_entity = new Dictionary<string, Entity>();
            entity_array = new List<Entity>();
        }

        /// <summary>
        /// Creates a number of empty entities of type "type" with each id in ids
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ids"></param>
        public Dictionary<string, Entity> CreateEntities(string type, string[] ids)
        {
            Dictionary<string, Entity> to_return = new Dictionary<string, Entity>();
            List<Entity> es = new List<Entity>();
            List<Entity> temp = entities.ContainsKey(type) ? entities[type] : AddSaverList(type);

            foreach(string id in ids)
            {
                Entity e = new Entity(id, entity_num);
                e.entity_type = type;
                es.Add(e);
                entity_num += 1;
            }

            temp.AddRange(es);

            foreach(Entity e in es)
            {
                to_return.Add(e.entity_string_id, e);
            }

            foreach (Entity e in es)
            {
                string_id_to_entity.Add(e.entity_string_id, e);
            }

            foreach(Entity e in es)
            {
                entity_array.Add(e);
            }

            //Debug.Log($"Created {ids.Length} entities");
            return to_return;
        }

        public Dictionary<string,Entity> GetEntities(string type, string[] ids)
        {
            Dictionary<string, Entity> temp = new Dictionary<string, Entity>(ids.Length);
            foreach (string s in ids)
            {
                temp.Add(s, string_id_to_entity[s]);
            }
            return temp;
        }

        public Dictionary<string, Entity> GetEntityDict(int[] ids)
        {
            Dictionary<string, Entity> to_return = new Dictionary<string, Entity>();
            foreach(int l in ids)
            {
                Entity e = entity_array[l];
                to_return.Add(e.entity_string_id, e);
            }
            return to_return;
        }

        public List<Entity> GetEntityList(int[] ids)
        {
            List<Entity> to_return = new List<Entity>();
            foreach(int id in ids)
            {
                to_return.Add(entity_array[id]);
            }
            return to_return;
        }

        List<Entity> AddSaverList(string type)
        {
            List<Entity> es = new List<Entity>();
            entities.Add(type, es);
            return es;
        }

        public Entity GetEntityFromId(string id)
        {
            if (string_id_to_entity.ContainsKey(id))
            {
                return string_id_to_entity[id];
            }
            Debug.LogWarning($"Could not find entity with id {id}");
            return null;
        }

        public bool CheckExistance(Entity e)
        {
            return string_id_to_entity.ContainsKey(e.entity_string_id);
        }

        public bool CheckExistance(string key)
        {
            return string_id_to_entity.ContainsKey(key);
        }

        public Entity this[string s]
        {
            get => GetEntityFromId(s);
        }

        /// <summary>
        /// This is gonna be super slow but it is for debugging
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string s = "";

            foreach(KeyValuePair<string,Entity> es in string_id_to_entity)
            {
                s += es.Key + " " + es.Value.ToString() + "\n";
                
            }

            return s;
        }

        public void DestroyEntity(Entity e)
        {
            entities[e.entity_type].Remove(e);
            entity_array.Remove(e);
            string_id_to_entity.Remove(e.entity_string_id);
        }
    }


}

