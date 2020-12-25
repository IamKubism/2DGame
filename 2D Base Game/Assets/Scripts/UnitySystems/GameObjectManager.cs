using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Psingine
{
    /// <summary>
    /// Scriptable object for handling unity game objects for entities
    /// </summary>
    public class GameObjectManager : MonoBehaviour
    {
        public static GameObjectManager instance;
        public Dictionary<Entity, GameObject> entity_objects;
        public Dictionary<GameObject, Entity> object_entities;

        private void Awake()
        {
            entity_objects = new Dictionary<Entity, GameObject>();
            object_entities = new Dictionary<GameObject, Entity>();
            if(instance == null)
            {
                instance = this;
            }
        }

        public Dictionary<Entity, GameObject> AddObjectsForEntities(List<Entity> entities)
        {
            //int i = 0;
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
                object_entities.Add(entity_go, e);
                //if (i < 10)
                //{
                //    //Debug.Log($"e: {e.entity_string_id} go: {entity_go.name}");
                //    i += 1;
                //}
            }
            watch.Stop();
            //Debug.Log($"Created {entities.Count} GameObjects in {watch.Elapsed}");
            return to_return;
        }

        public Dictionary<Entity, GameObject> AddComponentToObjects<T>(List<Entity> entities, Action<T, Entity> on_added) where T: Component
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

        public List<Entity> GetEntitiesFromObjects(List<GameObject> objects)
        {
            List<Entity> to_return = new List<Entity>();
            foreach (GameObject g in objects)
            {
                if (object_entities.ContainsKey(g))
                {
                    to_return.Add(object_entities[g]);
                }
            }
            return to_return;
        }

        public void RemoveEntity(Entity e)
        {
            GameObject obj = entity_objects[e];
            object_entities.Remove(obj);
            entity_objects.Remove(e);
            Destroy(obj);
        }

        public void RemoveEntities(List<Entity> es)
        {
            foreach(Entity e in es)
            {
                RemoveEntity(e);
            }
        }
    }
}

