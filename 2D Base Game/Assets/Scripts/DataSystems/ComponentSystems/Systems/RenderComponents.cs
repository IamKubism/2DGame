using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public class RenderStateManager : ISystemAdder
    {
        public static RenderStateManager instance;
        List<string> sprite_names;
        List<string> layer_names;
        Dictionary<string, RenderComponent> rendered_objects;
        Dictionary<string, RenderComponent> render_types;

        Dictionary<string, Entity> rendered_entities;

        Action<List<Entity>> on_added_entities;
        Action<List<Entity>> on_changed_entities;

        public RenderStateManager()
        {
            if(instance == null)
            {
                instance = this;
            } else
            {
                Debug.LogError("RenderStateManager already exists");
            }
            rendered_objects = new Dictionary<string, RenderComponent>();
            render_types = new Dictionary<string, RenderComponent>();
            PrototypeLoader.instance.AddSystemLoc("render_components", this);
            //Debug.Log("Made Render Manager");
            rendered_entities = new Dictionary<string, Entity>();
        }

        public void AddRenderedObject(string entity_id, string sprite_name, string layer_name, Positions pos)
        {
            if (rendered_objects.ContainsKey(entity_id))
            {
                Debug.LogError($"Tried to add a rendered entity twice: {entity_id}");
                return;
            }
            else
            {
                rendered_objects.Add(entity_id, new RenderComponent(pos.DisplacedVector(entity_id), layer_name, sprite_name));
            }
        }

        public void ChangeSpritePosition(Positions pos, string entity_id)
        {
            //I am planning on adding this to the update queue for the positions thing so I have to make sure the entity with a position is in this set
            rendered_objects[entity_id].position = pos.all_positions[entity_id].disp_pos;
        }

        public void RemoveRenderedObject(string entity_id)
        {
            if (rendered_objects.ContainsKey(entity_id))
            {
                rendered_objects.Remove(entity_id);
            }
            else
            {
                Debug.LogError($"Tried to remove unrendered object: {entity_id}");
            }
        }

        public RenderComponent GetComponent(string entity_id)
        {
            return rendered_objects[entity_id];
        }


        //
        // System Adder Interface implementation
        //

        public void AddEntities(List<Entity> entities)
        {
            foreach (Entity e in entities)
            {
                e.GetComponent<RenderComponent>("RenderComponent").position = e.GetComponent<Position>("Position").disp_pos;
                rendered_objects.Add(e.entity_string_id, e.GetComponent<RenderComponent>("RenderComponent"));
            }
            OnAddedEntities(entities);
            //Debug.Log($"Added {entities.Count} render components");
        }

        public void ChangeEntityValues(List<Entity> entities, RenderComponent vals)
        {
            foreach (Entity e in entities)
            {
                e.GetComponent<RenderComponent>("RenderComponent").SetStateValues(vals);
            }
            on_changed_entities(entities);
        }

        public void OnEntitiesChanged(List<Entity> entities)
        {
            on_changed_entities(entities);
        }

        public void OnAddedEntities(List<Entity> entities)
        {
            on_added_entities(entities);
        }

        public void RegisterOnAddedEntities(Action<RenderStateManager, List<Entity>> action)
        {
            on_added_entities += (entities) => { action(this, entities); };
        }

        public void RegisterOnChangedEntities(Action<RenderStateManager, List<Entity>> action)
        {
            on_changed_entities += (entities) => { action(this, entities); };
        }
    }
}