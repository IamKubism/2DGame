using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HighKings;

[JsonObject(MemberSerialization.OptIn)]
public class EntityPrototype
{
    [JsonProperty]
    string prototype_name;

    [JsonProperty]
    public Dictionary<string, ComponentInfo> component_info;

    public List<string> load_order;

    public Dictionary<string, object> component_values;

    public EntityPrototype(string prototype_name)
    {
        this.prototype_name = prototype_name;
        component_info = new Dictionary<string, ComponentInfo>();
        component_values = new Dictionary<string, object>();
        load_order = new List<string>();
    }

    public void ResetName(string prototype_name)
    {
        this.prototype_name = prototype_name;
    }

    public void SetComponent(string comp_name, int load_order, object o)
    {
        if (component_values.ContainsKey(comp_name))
        {
            component_values[comp_name] = o;
        } else
        {
            component_values.Add(comp_name, o);
        }
        
    }

    void OverwriteComponentInfo(ComponentInfo comp)
    {
        if (component_info.ContainsKey(comp.component_name))
        {
            component_info[comp.component_name] = comp;
        } else
        {
            component_info.Add(comp.component_name, comp);
        }
    }

    /// <summary>
    /// Full overwrite of data
    /// </summary>
    /// <param name="s"></param>
    /// <param name="o"></param>
    public void OverwriteComponentValue(string s, object o)
    {
        if (component_values.ContainsKey(s))
        {
            component_values[s] = o;
        } else
        {
            component_values.Add(s, o);
        }
        //Debug.Log($"Overwrote {s} {o.ToString()}");
    }

    /// <summary>
    /// Partial overwrite of data
    /// </summary>
    /// <param name="s"></param>
    /// <param name="o"></param>
    /// <param name="fields"></param>
    public void OverwriteComponentValue(string s, object o, List<FieldInfo> fields)
    {
        if (component_values.ContainsKey(s))
        {
            foreach (FieldInfo field in fields)
            {
                field.SetValue(component_values[s], field.GetValue(o));
            }
        } else
        {
            component_values.Add(s, o);
        }
    }
    
    public EntityPrototype Clone()
    {
        return new EntityPrototype(prototype_name)
        {
            component_info = new Dictionary<string, ComponentInfo>(component_info),
            component_values = new Dictionary<string, object>(component_values)
        };
    }

    public EntityPrototype Clone(string name)
    {
        return new EntityPrototype(name)
        {
            prototype_name = name,
            component_info = new Dictionary<string, ComponentInfo>(component_info),
            component_values = new Dictionary<string, object>(component_values)
        };
    }

    public override string ToString()
    {
        string s = prototype_name;

        foreach(ComponentInfo c in component_info.Values)
        {
            s += $"\n {c.ToString()} ";
            if (c.variable)
            {
                s += " variable";
            } else
            {
                s += $"{component_values[c.component_name].ToString()}";
            }
        }

        return s;
    }
}

/*  Tile prototype initialization pseudocode (for planning purposes)
 *  
 *  Defined in prototype:
 *  
 *  tile_id (to initialize the entity with a unique and nicely identifible name)
 *  defined_effect_name (base movement effect)
 *  sprite_name (for rendering)
 *  sprite_layer (for rendering)
 *  UniqueGenerator() (function based on initialization things to make the id of the entity unique) 
 *  
 *  Create(x, y, z):
 *      entity_id = tile_id + UniqueGenerator(x,y,z)
 *      positions.createposition(entity_id, x, y, z)
 *      node_effects.CreateNodeEffect(entity_id, defined_effect_name)
 *      node_map.CreateNode(entity_id, x, y, z)
 *      render_states.add(entity_id,sprite_name,sprite_layer,positions[entity_id])
 *      
 *      
 *  What this needs:
 *  1. There are arguments that are given to initialize the entity based on the kind of prototype
 *  2. It needs to know the systems that run the components
 *  3. It needs a generator for unique strings
 *  4. It needs to know what its parameters for initialization are
 *  
 *  
 *  Alternative Idea:
 *  
 *  StoneTileInstantiator:
 *  
 *  id_generator()
 *  Map<component_name,component_maker> components;
 *  Map<component_name,Map<arg_name,fixed_argument>> fixed_args;
 *  
 *  Create(variable_args):
 *      entity_id = id_generator(variable_args)
 *      foreach (component):
 *          args = make_args(component, fixed_args[component], variable_args[component])
 *          component.create(entity_id, Dict<string, obj> args)
 *      
 *  
 */

//[Serializable, JsonObject(MemberSerialization.OptIn)]
//public struct EntityPrototype
//{
//    private Dictionary<string, object> components;

//    [JsonProperty]
//    public string prototype_name;

//    /// <summary>
//    /// Where the components will be loaded
//    /// </summary>
//    [JsonProperty]
//    public Dictionary<string, string> system_names;

//    [JsonProperty]
//    public Dictionary<string, int> component_load_priorities;

//    public IDArguments id_args;

//    [JsonProperty]
//    public string id_generator_name;

//    public EntityPrototype(EntityPrototype p) : this()
//    {
//        //prototype_name = p.prototype_name;
//        System.IO.MemoryStream ms = new System.IO.MemoryStream();
//        BinaryFormatter b = new BinaryFormatter();
//        b.Serialize(ms, p);
//        ms.Position = 0;
//        this = (EntityPrototype)b.Deserialize(ms);
//    }

//    public static EntityPrototype operator +(EntityPrototype p, Tuple<string[], object> fields_comp)
//    {
//        if (p.id_args == null)
//        {
//            p.id_args = new IDArguments();
//        }
//        if (p.components == null)
//        {
//            p.components = new Dictionary<string, object>();
//        }
//        if (p.components.ContainsKey(fields_comp.Item2.GetType().ToString()))
//        {
//            return Overwrite(p, fields_comp);
//        }
//        else
//        {
//            p.components.Add(fields_comp.Item2.GetType().ToString(), fields_comp.Item2);
//        }
//        try
//        {
//            p.id_args.SetIDArgs((IIDArg)p.components[fields_comp.Item2.GetType().ToString()]);
//        }
//        catch
//        {

//        }

//        return p;
//    }

//    public static EntityPrototype Overwrite(EntityPrototype p, Tuple<string[], object> fields_over)
//    {
//        foreach (string s in fields_over.Item1)
//        {
//            //Debug.Log(fields_over.Item2.GetType().ToString() + " " + p.components[fields_over.Item2.GetType().ToString()].GetType().ToString());
//            FieldInfo f = fields_over.Item2.GetType().GetField(s);

//            f.SetValue(p.components[fields_over.Item2.GetType().ToString()], f.GetValue(fields_over.Item2));
//        }
//        try
//        {
//            Debug.Log(fields_over.Item2.GetType().ToString());
//            p.id_args.SetIDArgs((IIDArg)p.components[fields_over.Item2.GetType().ToString()]);
//        }
//        catch
//        {

//        }
//        return p;
//    }


//    public static EntityPrototype Overwrite(EntityPrototype p, object[] fields_over)
//    {
//        if(fields_over.Length != 3)
//        {
//            Debug.LogError($"Passed incorrect argument to overwrite for prototype {p.prototype_name}");
//            return p;
//        }
//        string comp_name;
//        string field_name;
//        try
//        {
//            comp_name = (string)fields_over[0];
//        } catch
//        {
//            Debug.LogError($"Component name not given a string for prototype {p.prototype_name}");
//            return p;
//        }
//        try
//        {
//            field_name = (string)fields_over[1];
//        } catch
//        {
//            Debug.LogError($"Field name for component {comp_name} not given a string for prototype {p.prototype_name}");
//            return p;
//        }
//        try
//        {
//            FieldInfo f = p.components[comp_name].GetType().GetField(field_name);
//            f.SetValue(p.components[comp_name], fields_over[2]);
//        } catch
//        {
//            Debug.LogError($"Could not overwrite field {field_name} for component {comp_name} on prototype {p.prototype_name}");
//        }
//        try
//        {
//            //Debug.Log((string)fields_over[0]);
//            p.id_args.SetIDArgs((IIDArg)p.components[(string)fields_over[0]]);
//        }
//        catch
//        {

//        }
//        return p;
//    }

//    public static EntityPrototype operator *(EntityPrototype prot, Dictionary<string,string> system_locs)
//    {
//        if(prot.system_names == null)
//        {
//            prot.system_names = new Dictionary<string, string>();
//        }
//        foreach (KeyValuePair<string,string> s in system_locs)
//        {
//            prot.system_names.Add(s.Key,s.Value);
//        }
//        return prot;
//    }

//    public override string ToString()
//    {
//        string s = "";
//        foreach (KeyValuePair<string,object> o in components)
//        {
//            s += o.Key + " " + o.Value.ToString() + " in " + system_names[o.Key] + "\n";
//        }
//        return s;
//    }

//    public object GetComponent(string comp_id)
//    {
//        return components[comp_id];
//    } 

//    public object[] GetComponents()
//    {
//        object[] comps = new object[components.Count];
//        int i = 0;
//        foreach(string s in components.Keys)
//        {
//            comps[i] = components[s];
//            i += 1;
//        }

//        return comps;
//    }
//}

///// <summary>
///// Get all the info that is relevant to this component type, index 0 is the prototype id, index 1 is the system id to load it to, index 2 is the load priority
///// </summary>
///// <param name="comp_type"></param>
///// <returns></returns>
//public object[] GetFixedComponentInfo(string comp_type)
//{
//    return new object[3] { GetComponentId(comp_type), GetComponentSys(comp_type), GetComponentPriority(comp_type) };
//}

//public void SetFixedComp(string comp_type, string comp_id, string system_loc, int load_priority)
//{
//    if(fixed_components.ContainsKey(comp_type) == false)
//    {
//        fixed_components.Add(comp_type, comp_id);
//        system_names.Add(comp_type, system_loc);
//        component_load_priorities.Add(comp_type, load_priority);
//    } else
//    {
//        fixed_components[comp_type] = comp_id;
//        system_names[comp_type] = system_loc;
//        component_load_priorities[comp_type] = load_priority;
//    }
//}

//public void SetFixedComps(Dictionary<string,string> comp_dict,
//    Dictionary<string,string> system_dict, Dictionary<string,int> comp_priorities)
//{
//    foreach(string comp_type in comp_dict.Keys)
//    {
//        SetFixedComp(comp_type, comp_dict[comp_type], system_dict[comp_type], comp_priorities[comp_type]);
//    }
//}

//public void SetVariableComps(List<string> comps,
//    Dictionary<string, string> system_dict, Dictionary<string, int> comp_priorities)
//{
//    foreach(string comp_type in comps)
//    {
//        SetVariableComp(comp_type, system_dict[comp_type], comp_priorities[comp_type]);
//    }
//}

//public void SetVariableComp(string comp_type, string system_loc, int load_priority)
//{
//    if (variable_components.Contains(comp_type))
//    {
//        system_names[comp_type] = system_loc;
//        component_load_priorities[comp_type] = load_priority;
//    } else
//    {
//        variable_components.Add(comp_type);
//        system_names.Add(comp_type, system_loc);
//        component_load_priorities.Add(comp_type, load_priority);
//    }
//}

//public string GetComponentId(string comp_type)
//{
//    return fixed_components.ContainsKey(comp_type) ? fixed_components[comp_type] : "NULL";
//}

//public string GetComponentSys(string comp_type)
//{
//    return system_names.ContainsKey(comp_type) ? system_names[comp_type] : "NULL";
//}

//public int GetComponentPriority(string comp_type)
//{
//    return component_load_priorities.ContainsKey(comp_type) ? component_load_priorities[comp_type] : 0x1000;
//}

///// <summary>
///// Loads one prototype into another, if the prototype is not extended self should be an empty prototype
///// </summary>
///// <param name="self"></param>
///// <param name="appender"></param>
//public static void LoadFromAppender(EntityPrototype self, EntityPrototype appender)
//{
//    //foreach(ComponentInfo c in appender.component_info.Values)
//    //{
//    //    self.OverwriteComponentInfo(c);
//    //}
//    //foreach(KeyValuePair<string,object> so in appender.component_values)
//    //{
//    //    self.OverwriteComponentValue(so.Key, so.Value);
//    //}
//}