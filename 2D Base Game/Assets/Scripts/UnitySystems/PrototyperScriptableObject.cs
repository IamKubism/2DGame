using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    /// <summary>
    /// Scriptable object made for creating the prototype loader for scriptable object stuff
    /// </summary>
    [CreateAssetMenu]
    public class PrototyperScriptableObject : ScriptableObject
    {
        public PrototypeLoader loader;
        public JsonParser parser;

        private void OnEnable()
        {
            parser = JsonParser.instance ?? new JsonParser();
            loader = PrototypeLoader.instance ?? new PrototypeLoader(parser);
        }
    }
}

