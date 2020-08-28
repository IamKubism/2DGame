using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PartGenerator<T>
{
    public T GeneratePart(string serial, JsonParser parser)
    {
        return parser.ParseString<T>(serial);
    }

    public void AddPartToSystem(object part, Dictionary<Type,Action<object>> putter_dict)
    {
        Type part_type = part.GetType();
        FieldInfo id_inf = part_type.GetField("string_id");
        putter_dict[part_type](id_inf.GetValue(part));
    }
}
