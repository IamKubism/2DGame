using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ComponentInfo
    {
        public List<Type> constructor_args;
        public ConstructorInfo construct;

        [JsonProperty]
        public string component_name;

        public bool errored = false;

        public object data;

        public ComponentInfo()
        {
            constructor_args = new List<Type>();
            errored = false;
        }

        public ComponentInfo(ComponentInfo info)
        {
            component_name = info.component_name;
            errored = info.errored;
            constructor_args = new List<Type>(info.constructor_args);
            construct = info.construct;
            ConstructorInfo constructor = info.data.GetType().GetConstructor(new Type[1] { info.data.GetType() });
            if (constructor == null)
            {
                Debug.LogError($"Could not find the copy constructor for {component_name}");
            }
            data = constructor.Invoke(new object[1] { info.data });
        }

        public ComponentInfo(JProperty comp_json, object generated_comp)
        {
            component_name = comp_json.Name;
            data = generated_comp;
            constructor_args = new List<Type>();

            if (comp_json.Value["constructor_args"] != null)
            {
                List<JToken> toks = comp_json.Value["constructor_args"].ToList();
                foreach (JToken tok in toks)
                {
                    Type temp = Type.GetType(PrototypeLoader.GenerateTypeName(tok.Value<string>("type"), tok.Value<string>("namespace")));
                    if (temp == null)
                    {
                        Debug.LogError($"Could not find type: {PrototypeLoader.GenerateTypeName(tok.Value<string>("type"), tok.Value<string>("namespace"))} for {comp_json.Name}");
                        errored = true;
                        continue;
                    }
                    constructor_args.Add(temp);
                }
            }

            constructor_args.Add(generated_comp.GetType());
            construct = generated_comp.GetType().GetConstructor(constructor_args.ToArray());

            if (construct == null)
            {
                string s = $"Could not find constructor for {comp_json.Name} with types (";
                foreach (Type t in constructor_args)
                {
                    s += $"{t.ToString()} ";
                }
                s += ")";
                Debug.LogError(s);
                errored = true;
            }
        }

        public ComponentInfo OverwriteComponentInfo(JProperty prop, object generated_obj)
        {
            ComponentInfo component_info = new ComponentInfo(this);

            if (prop.Value["constructor_args"] == null)
            {
            } else
            {
                component_info.constructor_args = new List<Type>();
                List<JToken> toks = prop.Value["constructor_args"].ToList();
                foreach (JToken tok in toks)
                {
                    Type temp = Type.GetType(PrototypeLoader.GenerateTypeName(tok.Value<string>("type"), tok.Value<string>("namespace")));
                    if (temp == null)
                    {
                        Debug.LogError($"Could not find type: {PrototypeLoader.GenerateTypeName(tok.Value<string>("type"), tok.Value<string>("namespace"))} for {prop.Name}");
                        errored = true;
                        continue;
                    }
                    component_info.constructor_args.Add(temp);
                }
                component_info.constructor_args.Add(data.GetType());
                component_info.construct = generated_obj.GetType().GetConstructor(constructor_args.ToArray());
                if (construct == null)
                {
                    string s = $"Could not find constructor for {prop.Name} with types (";
                    foreach (Type t in component_info.constructor_args)
                    {
                        s += $"{t.ToString() }";
                    }
                    s += ")";
                    Debug.LogError(s);
                    errored = true;
                }
            }

            component_info.data = component_info.data.GetType().GetConstructor(new Type[1] { component_info.data.GetType() }).Invoke(new object[1] { component_info.data });

            if(prop.Value["data"] != null)
            {
                List<FieldInfo> fields = new List<FieldInfo>();
                List<JToken> toks = prop.Value["data"].ToList();
                foreach(JProperty p in toks)
                {
                    FieldInfo f = generated_obj.GetType().GetField(p.Name);
                    if(f == null)
                    {
                        Debug.LogError($"Could not find field {p.Name} for component {component_info.component_name}");
                        continue;
                    }
                    fields.Add(f);
                }
                foreach(FieldInfo f in fields)
                {
                    f.SetValue(component_info.data, f.GetValue(generated_obj));
                }
            }

            return component_info;
        }

        public void SetData(object o, List<FieldInfo> fields)
        {
            if(data == null)
            {
                ConstructorInfo constructor = o.GetType().GetConstructor(new Type[1] { o.GetType() });
                if (constructor == null)
                {
                    constructor = o.GetType().GetConstructor(new Type[0]);
                    if (constructor == null)
                    {
                        Debug.LogError($"Could not find any base constructor for component: {component_name}");
                    } else
                    {
                        data = constructor.Invoke(new object[0]);
                    }
                } else
                    data = constructor.Invoke(new object[1] { o });
            } else
            {
                foreach(FieldInfo field in fields)
                {
                    field.SetValue(data, field.GetValue(o));
                }
            }
        }

        //public IBaseComponent CreateComponent(object[] args)
        //{
        //    if (errored)
        //    {
        //        return default;
        //    }

        //    ConstructorInfo constructor = variable ? data.GetType().GetConstructor(constructor_args.ToArray()) 
        //                                           : data.GetType().GetConstructor(new Type[1] { data.GetType() });

        //    if (constructor == null)
        //    {
        //        string s = $"Could not find correct constructor for {component_name} of type {component_type} with arguments:";
        //        foreach (Type t in constructor_args)
        //        {
        //            s += t.Name + " ";
        //        }
        //        Debug.LogError(s);
        //        return default;
        //    }

        //    object[] casted_args = args == null ? new object[0] : new object[args.Length];
        //    for(int i = 0; i < casted_args.Length; i += 1)
        //    {
        //        casted_args[i] = Convert.ChangeType(args[i], constructor_args[i]);
        //    }

        //    return (IBaseComponent)constructor.Invoke(casted_args);
        //}

        public override string ToString()
        {
            string s = $"Name: {component_name}, Value: {data.ToString()} with constructor args {{";
            foreach(Type t in constructor_args)
            {
                s += " " + t.ToString();
            }
            s += "}";
            return s;
        }
    }
}