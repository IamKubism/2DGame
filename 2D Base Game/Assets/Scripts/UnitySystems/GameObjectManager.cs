using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    /// <summary>
    /// Scriptable object for handling unity game objects for entities
    /// </summary>
    public class GameObjectManager : MonoBehaviour
    {
        public Dictionary<Entity, GameObject> entity_objects;

        private void Awake()
        {
            entity_objects = new Dictionary<Entity, GameObject>();

        }

        public Dictionary<Entity, GameObject> AddObjectsForEntities(List<Entity> entities)
        {
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            Dictionary<Entity, GameObject> to_return = new Dictionary<Entity, GameObject>();
            foreach (Entity e in entities)
            {
                if (entity_objects.ContainsKey(e))
                {
                    continue;
                }
                GameObject entity_go = new GameObject();
                entity_go.name = e.entity_string_id;
                entity_objects.Add(e, entity_go);
                to_return.Add(e, entity_go);
            }
            watch.Stop();
            Debug.Log($"Created {entities.Count} GameObjects in {watch.Elapsed}");
            return to_return;
        }

        public Dictionary<Entity, GameObject> AddComponentToObject<T>(List<Entity> entities, Action<T, Entity> on_added) where T: Component
        {
            Dictionary<Entity, GameObject> to_return = new Dictionary<Entity, GameObject>();
            foreach (Entity e in entities)
            {
                if(entity_objects.ContainsKey(e) == false)
                {
                    Debug.Log($"Could not find GameObject for {e.entity_string_id}, continuing");
                    continue;
                }
                GameObject entity_go = entity_objects[e];
                on_added(entity_go.AddComponent<T>(), e);
                to_return.Add(e, entity_go);
            }
            return to_return;
        }

        public Dictionary<Entity, GameObject> ActivateOnComponent<T>(List<Entity> entities, Action<T, Entity> activate) where T : Component
        {
            Dictionary<Entity, GameObject> to_return = new Dictionary<Entity, GameObject>();

            foreach (Entity e in entities)
            {
                if (entity_objects.ContainsKey(e) == false)
                {
                    Debug.Log($"Could not find GameObject for {e.entity_string_id}, continuing");
                    continue;
                }
                GameObject entity_go = entity_objects[e];
                T comp = entity_go.GetComponent<T>() == null ? entity_go.AddComponent<T>() : entity_go.GetComponent<T>();
                to_return.Add(e, entity_go);
                activate(comp, e);
            }

            return to_return;
        }
    }
}

