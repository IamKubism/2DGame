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
        /// <summary>
        /// For everything to reference, for ease of use
        /// </summary>
        public static EntityManager instance;

        /// <summary>
        /// This is used near entirely for creating nicer looking save files
        /// </summary>
        Dictionary<string, List<Entity>> entities;

        Dictionary<string, Entity> id_to_entity;

        public EntityManager()
        {
            if(instance != null)
            {
                Debug.LogError("Entity manager already exists");
            } else
            {
                instance = this;
            }
            entities = new Dictionary<string, List<Entity>>();
            id_to_entity = new Dictionary<string, Entity>();
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
                es.Add(new Entity(id));
            }
            temp.AddRange(es);
            foreach(Entity e in es)
            {
                to_return.Add(e.entity_string_id, e);
            }
            foreach (Entity e in es)
            {
                id_to_entity.Add(e.entity_string_id, e);
            }
            //Debug.Log($"Created {ids.Length} entities");
            return to_return;

        }

        public Dictionary<string,Entity> GrabEntities(string type, string[] ids)
        {
            Dictionary<string, Entity> temp = new Dictionary<string, Entity>(ids.Length);
            foreach (string s in ids)
            {
                temp.Add(s, id_to_entity[s]);
            }
            return temp;
        }

        List<Entity> AddSaverList(string type)
        {
            List<Entity> es = new List<Entity>();
            entities.Add(type, es);
            return es;
        }

        /// <summary>
        /// This is gonna be super slow but it is for 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string s = "";
            //foreach(KeyValuePair<string,List<Entity>> es in entities)
            //{
            //    s += es.Key + "\n";
            //    foreach(Entity e in es.Value)
            //    {
            //        s += $"\n {e.ToString()}";
            //    }
            //}
            foreach(KeyValuePair<string,Entity> es in id_to_entity)
            {
                s += es.Key + " " + es.Value.ToString() + "\n";
                
            }

            return s;
        }
    }


}

