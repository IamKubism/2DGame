using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentGenerator<T>
{
    public T GenThing(string json_string, JsonParser parser)
    {
        return parser.ParseString<T>(json_string);
    }
}
