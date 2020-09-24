using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class SelectionManager
    {
        public static SelectionManager instance;
        ComponentSubscriber<SelectionComponent> selects;
        public List<SelectionComponent> active_selectables;
        public Dictionary<Entity, SelectionComponent> selection_data;
        public GameObjectManager go_manager;

        public SelectionManager()
        {
            active_selectables = new List<SelectionComponent>();
            selection_data = new Dictionary<Entity, SelectionComponent>();
            if (instance == null)
            {
                instance = this;
            }
            selects = MainGame.instance.GetSubscriberSystem<SelectionComponent>("SelectionComponent");
            selects.RegisterOnAdded(AddEntities);
        }

        public void AddEntities(List<Entity> entities)
        {
            foreach (Entity e in entities)
            {
                if (selection_data.ContainsKey(e) == false)
                {
                    selection_data.Add(e, e.GetComponent<SelectionComponent>("SelectionComponent"));
                }
            }
            GameObjectManager.instance.AddComponentToObjects<BoxCollider>(entities, SetSelectionComponentValues);
        }

        /// <summary>
        /// TODO: Larger Entity types
        /// </summary>
        /// <param name="comp"></param>
        /// <param name="es"></param>
        void SetSelectionComponentValues(BoxCollider comp, Entity es)
        {
            comp.size = new Vector3(1f, 1f);
            comp.isTrigger = true;
        }
    }
}
