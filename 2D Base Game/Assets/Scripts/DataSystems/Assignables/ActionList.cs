using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    /// <summary>
    /// Class that manages all assignable actions
    /// </summary>
    public class ActionList
    {
        public static ActionList instance;
        Dictionary<string, EntityAction> all_actions;
        Dictionary<string, object> usable_params;

        public ActionList()
        {
            if(instance == null)
            {
                instance = this;
            }
            all_actions = new Dictionary<string, EntityAction>();
            usable_params = new Dictionary<string, object>();
        }

        public void CallAction(string id, Entity source, Entity target)
        {
            all_actions[id].Invoke(source, target);
        }

        public void RegisterAction(string id, EntityAction action)
        {
            if (all_actions.ContainsKey(id))
            {
                Debug.LogError($"Tried to register an action twice: {id}");
            }
            else
            {
                all_actions.Add(id, action);
            }
        }

        public void RegisterAction(string id)
        {
            if (all_actions.ContainsKey(id))
            {
                Debug.LogError($"Tried to register an action twice: {id}");
            } else
            {
                all_actions.Add(id, new EntityAction(id));
            }
        }

        public void RegisterListener(string id, EntityListener listener)
        {
            if (all_actions.ContainsKey(id))
            {
                all_actions[id].RegisterAction(listener);
            } else
            {
                all_actions.Add(id, new EntityAction(id, listener));
            }
        }

        public void UnRegisterAction(string id)
        {
            if (all_actions.ContainsKey(id))
            {
                all_actions.Remove(id);
            } else
            {
                Debug.LogError($"Tried to remove a non existant action: {id} ");
            }
        }

        public void UnRegisterListener(string id, string listener_id)
        {
            if (all_actions.ContainsKey(id))
            {
                all_actions[id].UnRegisterListener(listener_id);
            } else
            {
                Debug.LogError($"Tried to remove a listerener on a non existant action: {id} ");
            }
        }

        public EntityAction GetAction(string id)
        {
            if (all_actions.ContainsKey(id))
            {
                return all_actions[id];
            } else
            {
                Debug.LogError($"Could not find the entity action: {id}");
                return null;
            }
        }
    }
}
