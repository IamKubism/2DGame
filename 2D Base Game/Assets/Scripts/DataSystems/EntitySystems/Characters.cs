using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    /// <summary>
    /// This is a system designed to keep track of entities which are characters, I think I might need this but maybe I do not
    /// </summary>
    public class Characters
    {
        public Dictionary<string, Entity> active_characters;
        Action<List<Entity>> on_added_entities;

        public Characters()
        {
            active_characters = new Dictionary<string, Entity>();

        }

        public void AddEntities(List<Entity> entities)
        {
            foreach(Entity e in entities)
            {
                active_characters.Add(e.entity_string_id, e);
            }
        }

        public void OnAddedEntities(List<Entity> entities)
        {
            on_added_entities?.Invoke(entities);
        }

        public void RegisterOnAddedEntities(Action<object,List<Entity>> to_reg)
        {
            on_added_entities += (es) => { to_reg(this, es); };
        }
    }
}
