using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        Dictionary<string, List<EntityAction>> actions_by_tag;
        Dictionary<string, Func<Entity>> retrieval_actions;
        Dictionary<string, object> usable_params;

        public ActionList()
        {
            if(instance == null)
            {
                instance = this;
            }
            all_actions = new Dictionary<string, EntityAction>();
            actions_by_tag = new Dictionary<string, List<EntityAction>>();
            usable_params = new Dictionary<string, object>();
            retrieval_actions = new Dictionary<string, Func<Entity>>();
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

        public void RegisterListener(string id,string retrieval_action, EntityListener listener)
        {
            if (all_actions.ContainsKey(id))
            {
                all_actions[id].RegisterListener(listener);
            } else
            {
                all_actions.Add(id, new EntityAction(id,retrieval_action,  listener));
            }
        }

        public void RegisterRetrievalAction(string tag_name, Func<Entity> action)
        {
            if (retrieval_actions.ContainsKey(tag_name))
            {
                Debug.LogError($"Tried to register two retrieval action types");
            }
            else
            {
                retrieval_actions.Add(tag_name, action);
            }
        }

        public void RegisterRetrievalAction(JProperty prop)
        {
            if (retrieval_actions.ContainsKey(prop.Name))
            {
                Debug.LogError($"Tried to register two retrieval action types {prop.Name}");
            }
            else
            {
                string tag_name = prop.Name;
                JToken method_json = prop.Value["method"];
                if (method_json == null)
                {
                    Debug.LogError($"No method assigned for {tag_name}, cancelling assign");
                    return;
                }
                Type method_loc = Type.GetType(PrototypeLoader.GenerateTypeName(method_json.Value<string>("type"), method_json.Value<string>("namespace")));
                if(method_loc == null)
                {
                    Debug.LogError($"Could not find type: {PrototypeLoader.GenerateTypeName(method_json.Value<string>("type"), method_json.Value<string>("namespace"))}");
                    return;
                }
                MethodInfo method_info = method_loc.GetMethod(method_json.Value<string>("method_name"));
                if(method_info == null)
                {
                    Debug.LogError($"Could not find method {method_json.Value<string>("method_name")} for type {method_loc.ToString()}");
                }
                Entity method() { return (Entity)method_info.Invoke(null, null); }
                retrieval_actions.Add(tag_name, method);
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

        public static void AddParamType<T>(string obj_name)
        {
            if (instance.usable_params.ContainsKey(obj_name) == false)
            {
                instance.usable_params.Add(obj_name, default(T));
                return;
            }
        }

        public static void SetParam(string param_name, object value)
        {
            if (instance.usable_params.ContainsKey(param_name))
            {
                if(instance.usable_params[param_name].GetType() == value.GetType())
                {
                    instance.usable_params[param_name] = value;
                    return;
                } else
                {
                    Debug.LogError($"Tried to set {param_name} of type {instance.usable_params[param_name].GetType().ToString()} to incorrect type {value.GetType().ToString()}");
                    return;
                }
            } else
            {
                Debug.LogError($"Could not find action param: {param_name}");
                return;
            }
        }

        public static T GetParam<T>(string param_name)
        {
            if (instance.usable_params.ContainsKey(param_name))
            {
                if(typeof(T) == instance.usable_params[param_name].GetType())
                {
                    return (T)instance.usable_params[param_name];
                }
                else
                {
                    Debug.LogError($"Tried to get wrong type (type called: {typeof(T)}, type stored: {instance.usable_params[param_name].GetType()}) for param {param_name} ");
                    return default;
                }
            } else
            {
                Debug.LogError($"Could not find action param: {param_name}");
                return default;
            }
        }

        public void AddTagged(string tag, EntityAction action)
        {
            if (actions_by_tag.ContainsKey(tag))
            {
                if (actions_by_tag[tag].Contains(action))
                {
                    Debug.LogError($"Tried to add action to tag list twice. tag: {tag}, action: {action.action_id}");
                } else
                {
                    actions_by_tag[tag].Add(action);
                }
            } else
            {
                actions_by_tag.Add(tag, new List<EntityAction> { action });
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

        public List<EntityAction> GetActionsByTag(string tag)
        {
            if (actions_by_tag.ContainsKey(tag))
            {
                return actions_by_tag[tag];
            } else
            {
                Debug.LogError($"Could not find actions with tag: {tag}");
                return new List<EntityAction>(0);
            }
        }

        public Func<Entity> GetRetrievalAction(string action_name)
        {
            if (retrieval_actions.ContainsKey(action_name))
            {
                return retrieval_actions[action_name];
            } else
            {
                Debug.LogError($"Could not find action {action_name}");
                return null;
            }
        }
    }
}
