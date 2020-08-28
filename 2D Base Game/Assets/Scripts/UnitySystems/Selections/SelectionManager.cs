using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{

    public class SelectionManager : ISystemAdder
    {
        public static SelectionManager instance;
        public List<SelectionComponent> active_selectables;
        public Dictionary<Entity, SelectionComponent> selection_data;
        public GameObjectManager go_manager;

        private void OnEnable()
        {
            active_selectables = new List<SelectionComponent>();
            selection_data = new Dictionary<Entity, SelectionComponent>();
            if (instance == null)
            {
                instance = this;
            }
            PrototypeLoader.instance.AddSystemLoc("selection_manager", this);
        }

        public SelectionManager()
        {
            active_selectables = new List<SelectionComponent>();
            selection_data = new Dictionary<Entity, SelectionComponent>();
            if (instance == null)
            {
                instance = this;
            }
            PrototypeLoader.instance.AddSystemLoc("selection_manager", this);
        }

        public void AddEntities(List<Entity> entities)
        {
            foreach (Entity e in entities)
            {
                if (selection_data.ContainsKey(e) == false)
                    selection_data.Add(e, e.GetComponent<SelectionComponent>("SelectionComponent"));
            }
            GameObjectManager.instance.AddComponentToObjects<BoxCollider>(entities, SetSelectionComponentValues);
        }

        public void OnAddedEntities(List<Entity> entities)
        {

        }

        void SetSelectionComponentValues(BoxCollider comp, Entity es)
        {
            comp.size = new Vector3(1f, 1f);
            comp.isTrigger = true;
        }
    }
}
