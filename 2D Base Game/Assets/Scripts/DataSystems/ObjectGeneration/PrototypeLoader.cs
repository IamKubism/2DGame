/////////////////////////////////////////
/// Prototype loading for objects and components
/// Last Updated: Version 0.0.0 10/27/2020
/// Uses code from: https://rogerjohansson.blog/2008/02/28/linq-expressions-creating-objects/ for faster constructor calls
/////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;
using System.Linq;
using Priority_Queue;
using System.Reflection;
using HighKings;
using System.Linq.Expressions;

namespace HighKings
{
    /// <summary>
    /// Loads every entity prototype in the game.
    /// </summary>
    public class PrototypeLoader
    {
        public static PrototypeLoader instance;

        Type generator_type = typeof(ComponentGenerator<>);

        Dictionary<string, EntityPrototype> prototypes;
        Dictionary<string, ISystemAdder> system_adders;

        EventManager event_manager;
        Dictionary<string, Type> comp_types;
        Dictionary<string, object> base_component_generators;
        Dictionary<string, ConstructorInfo> jobject_constructors;
        Dictionary<string, object> base_component_defaults;
        JsonParser parser;
        Dictionary<Tuple<string, ConstructorInfo>, ObjectActivator> object_activators;

        public static BindingFlags field_flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Default;

        public PrototypeLoader(JsonParser parser)
        {
            if (instance == null)
            {
                instance = this;
            }
            this.parser = parser;
            comp_types = new Dictionary<string, Type>();
            system_adders = new Dictionary<string, ISystemAdder>();
            base_component_generators = new Dictionary<string, object>();
            base_component_defaults = new Dictionary<string, object>();
            prototypes = new Dictionary<string, EntityPrototype>();
            jobject_constructors = new Dictionary<string, ConstructorInfo>();
            object_activators = new Dictionary<Tuple<string, ConstructorInfo>, ObjectActivator>();
            event_manager = EventManager.instance ?? new EventManager();
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

            if (root["components"] != null)
            {
                List<JToken> comp_list = root["components"].ToList();
                foreach (JProperty comp in comp_list)
                {
                    CreateComponentType(comp);
                }
            }

            if (root["prototypes"] != null)
            {
                List<JToken> prot_types = root["prototypes"].ToList();
                foreach (JProperty prot in prot_types)
                {
                    CreateEntityPrototype(prot);
                }
            }

            if (root["actions"] != null)
            {
                List<JToken> action_types = root["actions"].ToList();
                foreach (JProperty act in action_types)
                {
                    CreateActionPrototype(act);
                }
            }

            if (root["retrieval_actions"] != null)
            {
                List<JToken> action_types = root["retrieval_actions"].ToList();
                foreach (JProperty prop in action_types)
                {
                    CreateRetrievalAction(prop);
                }
            }

            if (root["goals"] != null)
            {
                CreateGoalSet(root["goals"].ToList());
            }

            if(root["events"] != null)
            {
                List<JToken> toks = root["events"].ToList();
                foreach(JProperty prop in toks)
                {
                    CreateEventType(prop);
                }
            }

            watch.Stop();
            //Debug.Log($"Read file in {watch.Elapsed}");
        }

        void CreateComponentType(JProperty comp_data)
        {
            string type_name = GenerateTypeName(comp_data.Value["type"].ToString(), comp_data.Value["namespace"].ToString());
            string comp_name = comp_data.Name;

            if (base_component_generators.ContainsKey(comp_name) == false)
                AddComponentGenerator(comp_name, type_name);

            if (MainGame.instance.component_subscribers.ContainsKey(comp_name) == false)
                AddComponentSubscriber(type_name, comp_name);

            Type c_type = Type.GetType(type_name);

            if (c_type == null || (base_component_generators.ContainsKey(comp_name) == false))
            {
                Debug.LogError($"Couldn't find the component type {type_name} when making {comp_data.Name}");
                return;
            }

            object comp_gen = base_component_generators[comp_name];

            ConstructorInfo constructor = c_type.GetConstructor(new Type[1] { typeof(JObject) });

            if (constructor != null)
            {
                jobject_constructors.Add(comp_name, constructor);
                base_component_defaults.Add(comp_name, constructor.Invoke(new object[1] { comp_data.Value["data"] }));
            }
            else
            {
                base_component_defaults.Add(comp_name, comp_gen.GetType().GetMethod("GenThing").Invoke(comp_gen,
                    new object[2] { comp_data.Value["data"].ToString(), parser }));
            }

            if (comp_data.Value["inspector_display"] != null)
            {
                InspectorData disp = new InspectorData(comp_data);
                MainGame.instance.display_data.Add(comp_name, disp);
            }
        }

        void AddComponentGenerator(string comp_name, string type_name)
        {
            Type comp_type = Type.GetType(type_name);
            if (comp_type == null)
            {
                Debug.LogError($"Could not find component type {type_name}");
                return;
            }
            Type gen_type = generator_type.MakeGenericType(comp_type);
            base_component_generators.Add(comp_name, Activator.CreateInstance(gen_type));

            ConstructorInfo[] constructors = comp_type.GetConstructors();
            foreach(ConstructorInfo c in constructors)
            {
                object_activators.Add(new Tuple<string, ConstructorInfo>(comp_name, c), GetActivator(c));
            }
            //foreach(ConstructorInfo constructor in constructors)
            //{
            //    ParameterInfo[] pars = constructor.GetParameters();
            //    object[] key_args = new object[pars.Length+1];
            //    key_args[0] = comp_name;
            //    for(int i = 0; i < pars.Length; i += 1)
            //    {
            //        key_args[i + 1] = pars[i].ParameterType;
            //        key_args[i + 1] = pars[i].ParameterType;
            //    }
            //    object_activators.Add(key_args, GetActivator(constructor));
            //}
        }

        void AddComponentSubscriber(string type_name, string comp_name)
        {
            Type type = Type.GetType(type_name);
            if (type == null)
            {
                Debug.LogError($"Could not find component type {type_name}");
            }
            ConstructorInfo constructor = typeof(ComponentSubscriberSystem).GetConstructor(new Type[1] { typeof(string) });
            if (constructor == null)
            {
                string s = $"Could not find correct constructor for {comp_name} of type {type_name}";
                Debug.LogError(s);
            }
            MainGame.instance.AddComponentSubscribers(comp_name, constructor.Invoke(new object[1] { comp_name }));
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

        void CreateEntityPrototype(JProperty prot)
        {
            string prot_name = prot.Name;

            //Generate a fresh prototype
            EntityPrototype p = new EntityPrototype(prot_name);
            List<JToken> prot_sub = prot.Values().ToList();

            if (prot.Value["extends"] != null)
            {
                if (prototypes.ContainsKey(prot.Value["extends"].ToString()))
                {
                    p = prototypes[prot.Value["extends"].ToString()].Clone(prot_name);
                }
                else
                {
                    Debug.LogError($"Could not find the prototype {prot.Value["extends"].ToString()} that {prot_name} extends, skipping prototype");
                    return;
                }
            }

            if (prot.Value["components"] != null)
            {
                List<JToken> comps = prot.Value["components"].ToList();
                foreach (JProperty comp in comps)
                {
                    string comp_name = comp.Name;
                    Type comp_type;
                    object generated_comp;

                    if (p.components.ContainsKey(comp_name))
                    {
                        comp_type = p.components[comp_name].data.GetType();
                        generated_comp = p.components[comp_name].data;
                    }
                    else
                    {
                        if (base_component_defaults.ContainsKey(comp_name) == false)
                        {
                            Debug.LogError($"Could not find component: {comp_name} for prototype: {prot_name}");
                            continue;
                        }
                        else
                        {
                            comp_type = base_component_defaults[comp_name].GetType();
                            generated_comp = GetBaseComponent(comp_name);
                            p.SetComponent(comp, generated_comp);
                        }
                    }

                    if (comp.Value["data"] != null)
                    {
                        if (jobject_constructors.ContainsKey(comp.Name))
                        {
                            generated_comp = jobject_constructors[comp.Name].Invoke(new object[1] { comp.Value["data"] });
                        }
                        else
                        {
                            generated_comp = base_component_generators[comp_name].GetType().GetMethod("GenThing").Invoke(base_component_generators[comp_name],
                                            new object[2] { comp.Value["data"].ToString(), parser });
                        }
                    }

                    p.SetComponent(comp, generated_comp);
                }
            }
            prototypes.Add(prot_name, p);
        }

        public void CreateActionPrototype(JProperty act)
        {
            EntityAction action = new EntityAction(act);
            ActionList.instance.RegisterAction(act.Name, action);
            if (act.Value["tags"] == null)
            {
                Debug.LogError($"Action: {act.Name} has no tags");
            }
            else
            {
                List<JToken> tags = act.Value["tags"].ToList();
                if (tags.Count < 1)
                {
                    Debug.LogError($"Action: {act.Name} has no tags");
                }
                foreach (JToken tag in tags)
                {
                    ActionList.instance.AddTagged(tag.Value<string>(), action);
                }
            }
        }

        public void CreateRetrievalAction(JProperty prop)
        {
            ActionList.instance.RegisterRetrievalAction(prop);
        }

        public void CreateEventType(JProperty p)
        {
            EventManager.instance.AddEventPrototype(p);
        }

        public void CreateGoalPrototype(JProperty prop)
        {
            Type t = Type.GetType(GenerateTypeName(prop.Value["type"].ToString(), prop.Value["namespace"].ToString()));
            if (t == null)
            {
                Debug.LogError($"Could not find type: {GenerateTypeName(prop.Value["type"].ToString(), prop.Value["namespace"].ToString())}");
                return;
            }
            ConstructorInfo constructor = t.GetConstructor(new Type[1] { typeof(JProperty) });
            if (constructor == null)
            {
                Debug.LogError($"Could not find Json Constructor for {t.ToString()}");
                return;
            }
            IGoal g = (IGoal)constructor.Invoke(new object[1] { prop });
            FullGoalMap.instance.AddGoal(g);
        }

        public void CreateGoalSet(List<JToken> goals)
        {
            foreach (JProperty p in goals)
            {
                CreateGoalPrototype(p);
            }

            List<Tuple<string, string>> edges = new List<Tuple<string, string>>();
            foreach (JProperty p in goals)
            {
                if (p.Value["child_nodes"] != null)
                {
                    foreach (JToken j in p.Value["child_nodes"].ToList())
                    {
                        edges.Add(new Tuple<string, string>(p.Name, j.Value<string>()));
                    }
                }
                if (p.Value["parent_nodes"] != null)
                {
                    foreach (JToken j in p.Value["parent_nodes"].ToList())
                    {
                        edges.Add(new Tuple<string, string>(j.Value<string>(), p.Name));
                    }
                }
            }
            FullGoalMap.instance.AddEdgesToGraph(edges);
        }

        public void AttachPrototype(string prototype_id, Dictionary<Entity, Dictionary<string, object[]>> entities)
        {
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

            EntityPrototype p = prototypes[prototype_id];
            List<ISystemAdder> adders = new List<ISystemAdder>();
            List<ComponentInfo> comps = p.components.Values.ToList();

            //System.Diagnostics.Stopwatch w2 = System.Diagnostics.Stopwatch.StartNew();

            foreach (ComponentInfo info in comps)
            {
                //w2.Restart();
                if (!info.errored)
                {
                    foreach (KeyValuePair<Entity, Dictionary<string, object[]>> ekv in entities)
                    {
                        object[] dict_args = ekv.Value.ContainsKey(info.component_name) ? ekv.Value[info.component_name] : new object[0];
                        object[] args = new object[dict_args.Length + 1];
                        for(int i = 0; i < dict_args.Length; i += 1)
                        {
                            args[i] = dict_args[i];
                        }
                        args[dict_args.Length] = info.data;
                        //args[dict_args.Length] = info.data;
                        //object[] key_args = new object[args.Length + 1];
                        //key_args[0] = info.component_name;
                        //for(int i = 0; i < args.Length; i += 1)
                        //{
                        //    key_args[i + 1] = args[i].GetType();
                        //}

                        ////I'm thinking its possible this will get slow but idk it seems not that bad, and might be more dynamic and would clean up the prototyping
                        ////LambdaExpression lambda = Expression.Lambda(typeof(PrototypeUtils.ObjectActivator<>), newExp, param);
                        ConstructorInfo c = info.data.GetType().GetConstructor(Array.ConvertAll(args, item => item.GetType()));
                        if (c == null)
                        {
                            string s = $"Could not find correct constructor for: {ekv.Key.entity_string_id} with args:";
                            foreach (object o in args)
                            {
                                s += $" {o.ToString()}";
                            }
                            Debug.LogError(s);
                            continue;
                        }
                        ekv.Key.AddComponent(info.component_name, (IBaseComponent)c.Invoke(args));
                        
                        //TODO: Get this working
                        //ConstructorInfo c = info.data.GetType().GetConstructor(Array.ConvertAll(args, item => item.GetType()));
                        //if (c == null || !object_activators.TryGetValue(new Tuple<string, ConstructorInfo>(info.component_name,c), out ObjectActivator activator))
                        //{
                        //    string s = $"Could not find correct constructor for: {ekv.Key.entity_string_id} with args:";
                        //    foreach (object o in args)
                        //    {
                        //        s += $" {o.ToString()}";
                        //    }
                        //    Debug.LogError(s);
                        //    continue;
                        //}
                        //ekv.Key.AddComponent(info.component_name, (IBaseComponent)activator.Invoke(args));
                    }
                    if (system_adders.ContainsKey(info.component_name + "_subscriber"))
                        adders.Add(system_adders[info.component_name + "_subscriber"]);
                }
                //w2.Stop();
                //Debug.Log($"Added: {info.component_name} in {w2.Elapsed}");
            }

            //System.Diagnostics.Stopwatch w2 = System.Diagnostics.Stopwatch.StartNew();
            foreach (ISystemAdder sys in adders)
            {
                //w2.Restart();
                sys.AddEntities(entities.Keys.ToList());
                //w2.Stop();
                //Debug.Log($"Added component {sys.SysCompName()} in {w2.Elapsed}");
            }
            watch.Stop();
            Debug.Log($"Created {entities.Count} entities of type {prototype_id} in {watch.Elapsed}");
        }

        public ISystemAdder GetSystemById(string system_id)
        {
            return system_adders[system_id];
        }

        public object GetBaseComponent(string comp_name)
        {
            if (base_component_defaults.ContainsKey(comp_name))
            {
                return base_component_defaults[comp_name].GetType().GetConstructor(new Type[1] { base_component_defaults[comp_name].GetType() }).Invoke(new object[1] { base_component_defaults[comp_name] });
            }
            else
            {
                Debug.LogError($"Could not find default for component {comp_name}");
                return default;
            }
        }

        public static string GenerateTypeName(string comp_name, string namespace_name)
        {
            switch (namespace_name)
            {
                case "Default":
                    return typeof(PrototypeLoader).Namespace + "." + comp_name;
                default:
                    return namespace_name + "." + comp_name;
            }
        }

        public delegate object ObjectActivator(params object[] args);

        public static ObjectActivator GetActivator(ConstructorInfo ctor)
        {
            Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            ParameterExpression param =
                Expression.Parameter(typeof(object[]), "args");

            Expression[] argsExp =
                new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp =
                    Expression.ArrayIndex(param, index);

                Expression paramCastExp =
                    Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            NewExpression newExp = Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            LambdaExpression lambda =
                Expression.Lambda(typeof(ObjectActivator), newExp, param);

            //compile it
            ObjectActivator compiled = (ObjectActivator)lambda.Compile();
            return compiled;
        }

        public class TypeArrayComparer : IEqualityComparer<object[]>
        {
            public bool Equals(object[] x, object[] y)
            {
                if (x.Length != y.Length)
                {
                    return false;
                }
                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i] != y[i])
                    {
                        return false;
                    }
                }
                return true;
            }

            public int GetHashCode(object[] obj)
            {
                int result = 17;
                for (int i = 0; i < obj.Length; i++)
                {
                    unchecked
                    {
                        result = result * 23 + obj[i].GetHashCode();
                    }
                }
                return result;
            }
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

//Iterate through the prototype subfields
//foreach (JProperty sub in prot_sub)
//{
//    switch (sub.Name)
//    {
//        //Find any prototype that this has all the same components
//        case "extends":
//            if (prototypes.ContainsKey(sub.Value.ToString()))
//            {
//                //Debug.Log($"Cloning: {sub.Value.ToString()}");
//                p = prototypes[sub.Value.ToString()].Clone(prot_name);
//            }
//            else
//            {
//                Debug.LogError($"Could not find the prototype {sub.Value.ToString()} that {prot_name} extends, skipping prototype");
//                return;
//            }
//            break;
//        //Generate the component values/overrides
//        case "components":
//            List<JToken> comps = sub.Values().ToList();
//            foreach (JProperty comp in comps)
//            {
//                string comp_name = comp.Name;
//                string type_name;
//                if(comp.Value["component_type"] == null || comp.Value["_namespace"] == null)
//                {
//                    if(p.components.ContainsKey(comp_name))
//                        type_name = p.components[comp_name].component_type;
//                    else
//                    {
//                        Debug.LogError($"No typing information for {comp_name} for prototype: {prot_name}");
//                        return;
//                    }
//                } else
//                {
//                    type_name = GenerateTypeName(comp.Value["component_type"].ToString(), comp.Value["_namespace"].ToString());
//                }
//                Type comp_type = Type.GetType(type_name);
//                if (comp_type == null)
//                {
//                    Debug.Log($"Could not find component of type {type_name} for component named {comp_name}");
//                    continue;
//                }
//                if (MainGame.instance.component_subscribers.ContainsKey(comp_name + "_subscriber") == false)
//                {
//                    Debug.LogError($"Could not find subscriber system for {comp_name}, skipping");
//                    continue;
//                }
//                //Set which fields will be overwritten by the loader for the info (load priority, constructor types, ect)
//                List<FieldInfo> info_fields = new List<FieldInfo>();
//                List<JToken> fields = comp.Values().ToList();

//                foreach(JProperty prop in fields)
//                {
//                    if(prop.Name == "data" || prop.Name == "component_name" || prop.Name == "_namespace" || prop.Name == "component_type")
//                    {
//                        continue;
//                    }
//                    if(typeof(ComponentInfo).GetField(prop.Name, field_flags) == null)
//                    {
//                        Debug.LogError($"Could not find field: {prop.Name} for info of {comp_name} for {prot_name}");
//                        continue;
//                    }
//                    info_fields.Add(typeof(ComponentInfo).GetField(prop.Name, field_flags));
//                }

//                ComponentInfo info = JsonParser.instance.ParseString<ComponentInfo>(comp.Value.ToString());
//                info.component_name = comp_name;
//                info.component_type = type_name;

//                //If data is null, we know that it needs to get the base component's values.
//                if (info.data == default)
//                {
//                    info.SetData(base_component_defaults[comp_name], null);
//                }

//                List<FieldInfo> over_fields = new List<FieldInfo>();

//                //The "data" field defines the component values for this prototype, so if its there something is gonna get overwritten and I want to be able to do this in a 
//                //Non-stupid way that is nice to write. So I have to grab the fields present in the overwriter and then apply them to the data in the component
//                if (comp.Value["data"] != null)
//                {
//                    List<JProperty> comp_fields = comp.Value["data"].Values<JProperty>().ToList();
//                    foreach (JProperty f in comp_fields)
//                    {
//                        if (comp_type.GetField(f.Name, field_flags) == null)
//                        {
//                            Debug.LogError($"Could not find field {f.Name} for component {comp_type.Name}");
//                            continue;
//                        }
//                        over_fields.Add(comp_type.GetField(f.Name, field_flags));
//                    }

//                    object comp_genner = base_component_generators[type_name];
//                    object over_comp = comp_genner.GetType().
//                        GetMethod("GenThing").Invoke(comp_genner, new object[2] { comp.Value["data"].ToString(), parser });
//                    info.SetData(over_comp, over_fields);
//                }

//                p.SetComponent(info, info_fields, over_fields);
//            }
//            break;
//    }
//}

//I get scared of things ending up not removing properly so I do this so I don't get infinite loops
//while (i > 0)
//{
//    //This loads the component data into the entities
//    ComponentInfo comp = comps[i - 1];
//    //Debug.Log($"Loading Component: {comp.component_name}");
//    if (comp.variable)
//    {
//        Type[] t_args = Array.ConvertAll(entities.First().Value[comp.component_name], item => item.GetType());
//        ConstructorInfo constructor = Type.GetType(comp.data.GetType()).GetConstructor(t_args);
//        if (constructor == null)
//        {
//            string s = $"Could not find correct constructor for {comp.component_name} with arguments: {{";
//            foreach (Type t in t_args)
//            {
//                s += t.Name + " ";
//            }
//            Debug.LogError(s + "}");
//        }
//        foreach (Entity e in entities.Keys)
//        {
//            e.AddComponent(comp.component_name, (IBaseComponent)constructor.Invoke(entities[e][comp.component_name]));
//        }
//    }
//    else
//    {
//        ConstructorInfo constructor = Type.GetType(comp.component_type).GetConstructor(new Type[1]
//        { Type.GetType(comp.component_type) });
//        if (constructor == null)
//        {
//            string s = $"Could not find correct constructor for {comp.component_name} with fixed type arguments";
//            Debug.LogError(s);
//        }
//        foreach (Entity e in entities.Keys)
//        {
//            e.AddComponent(comp.component_name,
//                (IBaseComponent)constructor.Invoke(new object[1] { p.components[comp.component_name].data }));
//        }
//    }
//    //This adds the components to the subscriber systems
//    if (system_adders.ContainsKey(comp.component_name + "_subscriber"))
//        adders.Add(system_adders[comp.component_name + "_subscriber"]);
//    i -= 1;
//}

//if (comp_type == null)
//{
//    Debug.Log($"There was a problem typing {comp_name}");
//    continue;
//}
//if (MainGame.instance.component_subscribers.ContainsKey(comp_name + "_subscriber") == false)
//{
//    Debug.LogError($"Could not find subscriber system for {comp_name}, skipping");
//    continue;
//}


////Set which fields will be overwritten by the loader for the info (load priority, constructor types, ect)
//List<FieldInfo> info_fields = new List<FieldInfo>();
//List<JToken> fields = comp.Values().ToList();

//foreach (JProperty prop in fields)
//{
//    if (prop.Name == "data" || prop.Name == "component_name" || prop.Name == "_namespace" || prop.Name == "component_type")
//    {
//        continue;
//    }
//    if (typeof(ComponentInfo).GetField(prop.Name, field_flags) == null)
//    {
//        Debug.LogError($"Could not find field: {prop.Name} for info of {comp_name} for {prot_name}");
//        continue;
//    }
//    info_fields.Add(typeof(ComponentInfo).GetField(prop.Name, field_flags));
//}

//ComponentInfo info = JsonParser.instance.ParseString<ComponentInfo>(comp.Value.ToString());
//info.component_name = comp_name;
//info.component_type = type_name;

////If data is null, we know that it needs to get the base component's values.
//if (info.data == default)
//{
//    info.SetData(base_component_defaults[comp_name], null);
//}

//List<FieldInfo> over_fields = new List<FieldInfo>();

////The "data" field defines the component values for this prototype, so if its there something is gonna get overwritten and I want to be able to do this in a 
////Non-stupid way that is nice to write. So I have to grab the fields present in the overwriter and then apply them to the data in the component
//if (comp.Value["data"] != null)
//{
//    List<JProperty> comp_fields = comp.Value["data"].Values<JProperty>().ToList();
//    foreach (JProperty f in comp_fields)
//    {
//        if (comp_type.GetField(f.Name, field_flags) == null)
//        {
//            Debug.LogError($"Could not find field {f.Name} for component {comp_type.Name}");
//            continue;
//        }
//        over_fields.Add(comp_type.GetField(f.Name, field_flags));
//    }

//    object comp_genner = base_component_generators[type_name];
//    object over_comp = comp_genner.GetType().
//        GetMethod("GenThing").Invoke(comp_genner, new object[2] { comp.Value["data"].ToString(), parser });
//    info.SetData(over_comp, over_fields);
//}

//p.SetComponent(info, info_fields, over_fields);