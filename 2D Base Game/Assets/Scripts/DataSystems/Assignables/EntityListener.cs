using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace HighKings
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
            string type = PrototypeLoader.GenerateTypeName(prop.Value["type"].ToString(), prop.Value["namespace"].ToString());
            MethodInfo method = Type.GetType(type).GetMethod(prop.Value["method_name"].ToString(), new Type[2] { typeof(Entity), typeof(Entity) });
            action = (e1, e2) => { method.Invoke(null, new object[2] { e1, e2 }); };
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