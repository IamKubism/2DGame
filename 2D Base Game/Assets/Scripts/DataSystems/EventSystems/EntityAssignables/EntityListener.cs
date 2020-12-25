using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Linq;

namespace Psingine
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EntityListener
    {
        /// <summary>
        /// Id of the listener
        /// </summary>
        [JsonProperty]
        public string id;

        /// <summary>
        /// Function that will be called using the source and target entities when this listener is invoked
        /// </summary>
        Action<Entity, Entity> action;

        MethodInfo method;

        public int priority;

        /// <summary>
        /// Constructor for Entity listeners that is not based on a Json file, in general I think we shouldn't use this except possibly for testing
        /// </summary>
        /// <param name="id"></param>
        /// <param name="action"></param>
        public EntityListener(string id, Action<Entity,Entity> action)
        {
            this.id = id;
            this.action = action;
        }

        /// <summary>
        /// Constructor that is used to create an EntityListener using a Json file and reflection, requires the property to define the function called be a static function
        /// </summary>
        /// <param name="prop"></param>
        public EntityListener(JProperty prop)
        {
            id = prop.Name;

            if(prop.Value["priority"] != null)
            {
                priority = prop.Value["priority"].Value<int>();
            } else
            {
                priority = 100;
            }

            //Get it to find the method to call
            string type = PrototypeLoader.GenerateTypeName(prop.Value["type"].ToString(), prop.Value["namespace"].ToString());
            MethodInfo method = Type.GetType(type).GetMethod(prop.Value["method_name"].ToString(), new Type[2] { typeof(Entity), typeof(Entity) });
            action = (e1, e2) => { method.Invoke(null, new object[2] { e1, e2 }); };

            //If a method requires some external params (such as a name of a furniture type to instantiate) we need to make a place for it to get the value, so we do that here
            if(prop.Value["params"] != null)
            {
                List<JToken> pars = prop.Value["params"].Values().ToList();
                foreach (JProperty p in pars)
                {
                    Type t = Type.GetType(PrototypeLoader.GenerateTypeName(p.Value["type"].ToString(), p.Value["namespace"].ToString()));
                    MethodInfo add_param_info = typeof(ActionList).GetMethod(nameof(ActionList.AddParamType), new Type[1] { typeof(string) }).MakeGenericMethod(t);
                    add_param_info.Invoke(null, new object[1] { p.Name });
                }
            }
        }

        /// <summary>
        /// Invoke the action loaded into this listener
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void Invoke(Entity source, Entity target)
        {
            action(source, target);
        }
    }

    public class EntityListener<T>
    {
        /// <summary>
        /// Id of the listener
        /// </summary>
        [JsonProperty]
        public string id;

        /// <summary>
        /// Function that will be called using the source and target entities when this listener is invoked
        /// </summary>
        Func<Entity, Entity, T> action;

        public int priority;

        /// <summary>
        /// Constructor for Entity listeners that is not based on a Json file, in general I think we shouldn't use this except possibly for testing
        /// </summary>
        /// <param name="id"></param>
        /// <param name="action"></param>
        public EntityListener(string id, Func<Entity, Entity, T> action)
        {
            this.id = id;
            this.action = action;
        }

        /// <summary>
        /// Constructor that is used to create an EntityListener using a Json file and reflection, requires the property to define the function called be a static function
        /// </summary>
        /// <param name="prop"></param>
        public EntityListener(JProperty prop)
        {
            id = prop.Name;

            if (prop.Value["priority"] != null)
            {
                priority = prop.Value["priority"].Value<int>();
            }
            else
            {
                priority = 100;
            }

            //Get it to find the method to call
            string type = PrototypeLoader.GenerateTypeName(prop.Value["type"].ToString(), prop.Value["namespace"].ToString());
            MethodInfo method = Type.GetType(type).GetMethod(prop.Value["method_name"].ToString(), new Type[2] { typeof(Entity), typeof(Entity) });
            action = (e1, e2) => { return (T)method.Invoke(null, new object[2] { e1, e2 }); };

            //If a method requires some external params (such as a name of a furniture type to instantiate) we need to make a place for it to get the value, so we do that here
            if (prop.Value["params"] != null)
            {
                List<JToken> pars = prop.Value["params"].Values().ToList();
                foreach (JProperty p in pars)
                {
                    Type t = Type.GetType(PrototypeLoader.GenerateTypeName(p.Value["type"].ToString(), p.Value["namespace"].ToString()));
                    MethodInfo add_param_info = typeof(ActionList).GetMethod(nameof(ActionList.AddParamType), new Type[1] { typeof(string) }).MakeGenericMethod(t);
                    add_param_info.Invoke(null, new object[1] { p.Name });
                }
            }
        }

        /// <summary>
        /// Invoke the action loaded into this listener
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void Invoke(Entity source, Entity target)
        {
            action(source, target);
        }

    }
}