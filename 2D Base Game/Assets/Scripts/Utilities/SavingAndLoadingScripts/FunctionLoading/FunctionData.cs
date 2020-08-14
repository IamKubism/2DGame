using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

/// <summary>
/// A class that gives values of a function with a relevant array of values that will be used in the parsing of the function
/// The parsing of values will be done by the interpreter
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public struct FunctionData
{
    /// <summary>
    /// Name of the function
    /// </summary>
    [JsonProperty]
    public readonly string script_name;

    /// <summary>
    /// For convenience of referencing different values by their name
    /// </summary>
    [JsonProperty]
    public readonly Dictionary<string, string> value_pairs;
}
