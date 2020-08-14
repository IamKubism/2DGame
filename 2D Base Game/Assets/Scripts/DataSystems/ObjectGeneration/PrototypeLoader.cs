﻿using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;
using System.Linq;
using Priority_Queue;
using System.Reflection;
using HighKings;

namespace HighKings
{
    /// <summary>
    /// Loads every entity prototype in the game.
    /// </summary>
    public class PrototypeLoader
    {
        public static PrototypeLoader instance;

        Type generator_type = typeof(ComponentGenerator<>);

        //Dictionary<string, EntityPrototype> all_prototypes;
        Dictionary<string, EntityPrototype> prototypes;
        Dictionary<string, ISystemAdder> system_adders;

        Dictionary<string, Type> comp_types;
        Dictionary<string, object> base_component_generators;
        Dictionary<string, object> base_component_defaults;
        JsonParser parser;

        public PrototypeLoader(JsonParser parser)
        {
            if (instance == null)
            {
                instance = this;
            }
            this.parser = parser;
            //all_prototypes = new Dictionary<string, EntityPrototype>();
            comp_types = new Dictionary<string, Type>();
            system_adders = new Dictionary<string, ISystemAdder>();
            base_component_generators = new Dictionary<string, object>();
            base_component_defaults = new Dictionary<string, object>();
            prototypes = new Dictionary<string, EntityPrototype>();
        }

        public void ReadFile(string file_text)
        {
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            if (base_component_defaults == null)
            {
                Debug.LogError("Component defaults was null");
                base_component_defaults = new Dictionary<string, object>();
            }
            if (base_component_generators == null)
            {
                Debug.LogError("Component Generators was null");
                base_component_generators = new Dictionary<string, object>();
            }
            if (comp_types == null)
            {
                Debug.LogError("Component types was null");
                comp_types = new Dictionary<string, Type>();
            }

            //Parse and iterate through the file
            JObject root = JObject.Parse(file_text);
            List<JToken> branch_list = root.Children().ToList();

            foreach (JProperty main_branch in branch_list)
            {
                switch (main_branch.Name)
                {
                    //Make the Default components defined in the file
                    case "components":
                        List<JToken> comp_list = main_branch.Value.ToList();
                        foreach (JProperty comp in comp_list)
                        {
                            string type_name = ((JProperty)comp.Values().ToList()[0]).Value.ToString();
                            if (base_component_generators.ContainsKey(type_name) == false)
                                AddComponentGenerator(type_name);
                            Type c_type = Type.GetType( this.GetType().Namespace + "." + type_name);
                            if (c_type == null || (base_component_generators.ContainsKey(type_name) == false))
                            {
                                Debug.LogError($"Couldn't find the component type {type_name} when making {comp.Name}");
                                continue;
                            }
                            object comp_gen = base_component_generators[type_name];
                            base_component_defaults.Add(comp.Name, comp_gen.GetType().GetMethod("GenThing").Invoke(comp_gen,
                                new object[2] { ((JProperty)comp.Values().ToList()[1]).Value.ToString(), parser }));
                        }
                        break;
                    //Make the prototypes
                    case "prototypes":
                        List<JToken> prot_types = main_branch.Values().ToList();
                        foreach (JProperty prot in prot_types)
                        {
                            CreatePrototype(prot);
                        }
                        break;
                }
            }
            watch.Stop();
            Debug.Log($"Read file in {watch.ElapsedTicks} ticks");
        }

        void AddComponentGenerator(string type_name)
        {
            Type comp_type = Type.GetType(this.GetType().Namespace + "." + type_name);
            if (comp_type == null)
            {
                Debug.LogError($"Could not find component type {type_name}");
            }
            Type gen_type = generator_type.MakeGenericType(comp_type);
            base_component_generators.Add(type_name, Activator.CreateInstance(gen_type));
        }


        public void AddSystemLoc(string sys_name, ISystemAdder sys)
        {
            if (system_adders.ContainsKey(sys_name))
            {
                Debug.LogError($"Prototype loader already has a location for {sys_name}");
                return;
            }
            system_adders.Add(sys_name, sys);
        }

        /*  What I would PREFER this does
         *  1. Copies components to the system they are added to rather than copying to the system every time a prototype is loaded
         *  2. Prototypes stored as a list of component names to reference rather than the actual component
         *  3. Systems are asked to copy their components by name to the table rather than copy individual objects each time
         * 
         *  Questions:
         *  How do we show that certain fields must be filled in at entity instantiation time, and are not fixed on prototype
         *  declaration? (position for example)
         * 
         * 
         */
        void CreatePrototype(JProperty prot)
        {
            string prot_name = prot.Name;
            //Generate a fresh prototype
            EntityPrototype p1 = new EntityPrototype(prot_name);
            EntityPrototype p = parser.ParseString<EntityPrototype>(prot.Value.ToString());
            List<JToken> prot_sub = prot.Values().ToList();

            //Iterate through the prototype subfields
            foreach (JProperty sub in prot_sub)
            {
                switch (sub.Name)
                {
                    //Find any prototype that this has all the same components
                    case "extends":
                        if (prototypes.ContainsKey(sub.Value.ToString()))
                        {
                            p1 = prototypes[sub.Value.ToString()].Clone();
                            EntityPrototype.LoadFromAppender(p, p1);
                        }
                        else
                        {
                            Debug.LogError("TODO: Implement extension prototype search");
                        }
                        break;
                    //Generate the component values/overrides
                    case "components":
                        List<JToken> comps = sub.Values().ToList();
                        foreach (JProperty comp in comps)
                        {
                            Type comp_type = Type.GetType(this.GetType().Namespace + "." + comp.Name);
                            if (comp_type == null)
                            {
                                Debug.Log("TODO: Search for component type");
                            }
                            //Debug.Log($"Parsing: {comp.Value.ToString()}");
                            object comp_genner = base_component_generators[comp.Name];
                            object over_comp = comp_genner.GetType().
                                GetMethod("GenThing").Invoke(comp_genner, new object[2] { comp.Value.ToString(), parser });
                            p.OverwriteComponentValue(comp.Name, over_comp);

                        }
                        break;
                }
            }
            prototypes.Add(prot_name, p);
            Debug.Log(p.ToString());
        }

        public void AttachPrototype(string prototype_id, Dictionary<Entity, Dictionary<string, object[]>> entities)
        {
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            SimplePriorityQueue<ComponentInfo> load_queue = new SimplePriorityQueue<ComponentInfo>();
            EntityPrototype p = prototypes[prototype_id];
            foreach (ComponentInfo c in p.component_info.Values)
            {
                load_queue.Enqueue(c, c.load_priority);
            }
            int i = load_queue.Count;
            while (load_queue.Count > 0 && i > 0)
            {
                watch.Restart();
                ComponentInfo comp = load_queue.Dequeue();
                Debug.Log($"Making {comp.component_name}");
                if (comp.variable)
                {
                    Type[] t_args = Array.ConvertAll(entities.First().Value[comp.component_name], item => item.GetType());
                    ConstructorInfo constructor = Type.GetType(this.GetType().Namespace + "." + comp.component_type).GetConstructor(t_args);
                    if (constructor == null)
                    {
                        string s = $"Could not find correct constructor for {comp.component_name} with arguments:";
                        foreach (Type t in t_args)
                        {
                            s += t.Name + " ";
                        }
                        Debug.LogError(s);
                    }
                    foreach (Entity e in entities.Keys)
                    {
                        e.AddComponent(comp.component_name, (IBaseComponent)constructor.Invoke(entities[e][comp.component_name]));
                    }
                    watch.Stop();
                    Debug.Log($"Added variable components in {watch.Elapsed}");
                }
                else
                {
                    ConstructorInfo constructor = Type.GetType(this.GetType().Namespace + "." + comp.component_type).GetConstructor(new Type[1]
                    { Type.GetType(this.GetType().Namespace + "." + comp.component_type) });
                    if (constructor == null)
                    {
                        string s = $"Could not find correct constructor for {comp.component_name} with fixed type argumemts";
                        Debug.LogError(s);
                    }
                    foreach (Entity e in entities.Keys)
                    {
                        e.AddComponent(comp.component_name,
                            (IBaseComponent)constructor.Invoke(new object[1] { p.component_values[comp.component_name] }));
                    }
                    watch.Stop();
                    Debug.Log($"Added components in {watch.Elapsed}");
                }
                if (comp.system_location != "None")
                    system_adders[comp.system_location].AddEntities(entities.Keys.ToList());
                i -= 1;
            }
            watch.Stop();
            Debug.Log($"Created {entities.Count} entities of type {prototype_id} in {watch.Elapsed}");
        }

        public ISystemAdder GetSystemById(string system_id)
        {
            return system_adders[system_id];
        }
    }
}


/*
[JsonObject(MemberSerialization.OptIn)]

        //comp2 = (TestDataStruct)comp1.Clone();
        //TestDataStruct tempdat = JsonConvert.DeserializeObject<TestDataStruct>(((JProperty)comps[1]).Value.ToString());
        //List<JToken> vals = dat.Values().ToList();
        //List<FieldInfo> fs = new List<FieldInfo>();
        //foreach(JProperty prop in vals)
        //{
        //    FieldInfo d = comp2.GetType().GetField("state").FieldType.GetField(prop.Name);
        //    fs.Add(d);
        //}
public class TestDataStruct : GetsPut, ICloneable
{
    [JsonObject(MemberSerialization.OptIn)]
    public struct State
    {
        [JsonProperty]
        public string st;

        [JsonProperty]
        public int n;

        [JsonProperty]
        public float f;

        public static State OverWrite(State b_data, State over_dat, List<FieldInfo> fs)
        {
            foreach(FieldInfo f in fs)
            {
                f.SetValue(b_data, f.GetValue(over_dat));
                Debug.Log(f.GetValue(b_data));
                Debug.Log(f.GetValue(over_dat));
            }

            return b_data;
        }

        [JsonProperty]
        InternalStruct ins;
    }

    public TestDataStruct(string s, int n, float f)
    {
        state = new State { st = s, n = n, f = f };
    }

    public string st
    {
        get
        {
            return state.st;
        }
    }

    public int n
    {
        get
        {
            return state.n;
        }
    }

    public float f
    {
        get
        {
            return state.f;
        }
    }

    [JsonProperty]
    public State state;

    delegate void ActionByRef<T1, T2>(ref T1 p1, ref T2 p2);
    delegate void ActionByRef<T1, T2, T3>(ref T1 p1, ref T2 p2, ref T3 p3);
    public delegate void MyDelegate();

    void UpdateState<TP1>(ActionByRef<State, TP1> proc, ref TP1 p1)
    { proc(ref state, ref p1); }
    void UpdateState<TP1, TP2>(ActionByRef<State, TP1, TP2> proc, ref TP1 p1, ref TP2 p2)
    { proc(ref state, ref p1, ref p2); }


    public void PutIn(object where_to_put)
    {
        ((List<TestDataStruct>)where_to_put).Add(this);
    }

    //public static TestDataStruct operator +(TestDataStruct x, int a)
    //{
    //    x.n += a;
    //    return x;
    //}

    public override string ToString()
    {
        return $"{st}, {n} {f} ";
    }

    public void OverWriteData(TestDataStruct over_data, List<FieldInfo> fs)
    {
        UpdateState((ref State s, ref TestDataStruct da, ref List<FieldInfo> fo) =>
        {
            //Debug.Log(da.ToString());
            object temps = s;
            foreach(FieldInfo f in fo)
            {
                //Debug.Log(f.GetValue(da.state));
                f.SetValue(temps, f.GetValue(da.state));
            }
            s = (State)temps;
        }, ref over_data, ref fs);
    }

    public object Clone()
    {
        TestDataStruct t = new TestDataStruct(st, n, f);
        return t;
    }
}
*/

//List<JToken> comps = sub.Values().ToList();
//foreach (JProperty comp in comps)
//{
//    Type comp_type = Type.GetType(comp.Name);

//    if (comp_type == null)
//    {
//        Debug.Log("TODO: Make the subprototype search method");
//        Debug.LogError($"Could not find component {comp.Name} for prototype {prot.Name}");
//    }

//    object comp_genner = base_component_generators[comp_type.ToString()];
//    object over_comp = comp_genner.GetType().
//        GetMethod("GenThing").Invoke(comp_genner, new object[2] { comp.Value.ToString(), parser });
//    int i = 0;
//    foreach (JProperty prop in comp.Values().ToList())
//    {
//        i += 1;
//    }
//    string[] fields = new string[i];
//    i = 0;
//    foreach (JProperty prop in comp.Values().ToList())
//    {
//        fields[i] = prop.Name;
//        i += 1;
//    }
//    p += new Tuple<string[], object>(fields, over_comp);
//}

//if (all_prototypes.ContainsKey(sub.Value.ToString()))
//{
//    p = new EntityPrototype(all_prototypes[sub.Value.ToString()]);
//}
//else
//{
//    Debug.Log("TODO: Implement extention prototype search");
//    Debug.LogError($"Could not find extention prototype: {prot.Name}, there are gonna be huge problems for anything loading that");
//}