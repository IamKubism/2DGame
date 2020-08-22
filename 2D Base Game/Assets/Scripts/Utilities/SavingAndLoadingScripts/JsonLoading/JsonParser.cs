using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System;

/// <summary>
/// This class is designed to returned parsed objects from JSON objects, it will also probably handle a lot of file loading in the future
/// </summary>
public class JsonParser
{
    JsonSerializerSettings config;

    public static JsonParser instance;

    public JsonParser()
    {
        if (instance == null)
        {
            instance = this;
        }
        config = new JsonSerializerSettings
        {

            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            MissingMemberHandling = MissingMemberHandling.Ignore, //Might want to change this
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

    }

    /// <summary>
    /// Returns a parsed type T from a string jsonText
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="jsonText"></param>
    /// <returns></returns>
    public T ParseString<T>(string jsonText)
    {
        //Debug.Log(jsonText);
        try
        {
            return JsonConvert.DeserializeObject<T>(jsonText, config);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.GetType().Name + ": " + ex.Message);
            return default;
        }
    }

    /// <summary>
    /// Appends a parsing of jsonText to a dictionary baseDict
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="jsonText"></param>
    /// <param name="baseDict"></param>
    public void AppendDictionary<T>(string jsonText, Dictionary<string,T> baseDict)
    {
        Dictionary<string, T> toAppend;
        try
        {
            toAppend = JsonConvert.DeserializeObject<Dictionary<string, T>>(jsonText, config);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.GetType().Name + ": " + ex.Message);
            return;
        }
        foreach (string s in toAppend.Keys)
        {
            if(baseDict == null)
            {
                baseDict = new Dictionary<string, T>();
            }
            if (baseDict.ContainsKey(s))
            {
                baseDict[s] = toAppend[s];
            } else
            {
                baseDict.Add(s, toAppend[s]);
            }
        }
    }

}

[JsonObject(MemberSerialization.OptIn)]
public class LoadPath
{
    [JsonProperty]
    public string Type;

    [JsonProperty]
    List<string> Path;

    public string filePath;

    /// <summary>
    /// Default Constructor, does nothing but makes an instance
    /// </summary>
    public LoadPath()
    {
        Path = new List<string>();
    }

    public string MakePathFromRoot(string root)
    {
        string path = root;
        foreach (string s in Path)
        {
            path = System.IO.Path.Combine(path, s);
        }
        return path;
    }


}