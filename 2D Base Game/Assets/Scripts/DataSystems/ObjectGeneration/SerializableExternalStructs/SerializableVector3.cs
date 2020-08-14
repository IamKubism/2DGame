using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This uses code copied from a unity answers thread:
/// https://answers.unity.com/questions/956047/serialize-quaternion-or-vector3.html
/// Made by "Cherno"
/// </summary>
[Serializable, JsonObject(MemberSerialization.OptIn)]
public struct SerializableVector3
{
    [JsonProperty]
    float x;

    [JsonProperty]
    float y;

    [JsonProperty]
    float z;

    public SerializableVector3(float x, float y, float z) : this()
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    /// Returns a string representation of the object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return string.Format("[{0}, {1}, {2}]", x, y, z);
    }

    /// <summary>
    /// Automatic conversion from SerializableVector3 to Vector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator Vector3(SerializableVector3 rValue)
    {
        return new Vector3(rValue.x, rValue.y, rValue.z);
    }

    /// <summary>
    /// Automatic conversion from Vector3 to SerializableVector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator SerializableVector3(Vector3 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, rValue.z);
    }
}