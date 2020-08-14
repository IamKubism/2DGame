using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighKings;

public class SelectionManager : ScriptableObject, ISystemAdder
{
    public List<SelectionComponent> ActiveSelectables;
    public Dictionary<Entity, SelectionComponent> selection_data;
    public GameObjectManager go_manager;

    private void OnEnable()
    {
    }

    public void AddEntities(List<Entity> entities)
    {
        foreach(Entity e in entities)
        {
            if(selection_data.ContainsKey(e) == false)
                selection_data.Add(e, e.GetComponent<SelectionComponent>("SelectionComponent"));
        }
        go_manager.AddComponentToObject<SelectionComponent>(entities, SetSelectionComponentValues);
    }

    public void OnAddedEntities(List<Entity> entities)
    {

    }

    void SetSelectionComponentValues(SelectionComponent comp, Entity es)
    {

    }
}
