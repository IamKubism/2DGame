using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace HighKings
{
    public class TextParsing
    {
        public static string ParseComponentText(string to_parse, IBaseComponent b, char sep_1 = '[', char sep_2 = ']')
        {
            string s = "";
            string[] p = to_parse.Split(sep_1);
            Type t_b = b.GetType();
            int i = 0;
            s += p[i];
            while(i < p.Length)
            {
                i += 1;
                if (p[i].Length == 0)
                {
                    continue;
                }
                string[] l = p[i].Split(sep_2);
                if(l.Length == 2)
                {
                    FieldInfo f = t_b.GetField(l[0]);
                    if(f == null)
                    {
                        Debug.LogWarning($"Parsing error for {b.GetType().ToString()} {l[0]}");
                        s += p[i];
                        continue;
                    }
                    s += f.GetValue(b).ToString();
                }
                s += l[l.Length - 1];
            }
            return s;
        }

        public static string ParseEntityText(string to_parse, Entity e, string sep_1 = "[", string sep_2 = "]")
        {
            string s = "";
            string[] p = to_parse.Split(new string[1] { sep_1 }, StringSplitOptions.None);
            int i = 0;
            s += p[i];
            while(i < p.Length)
            {
                i += 1;
                if(p[i].Length == 0)
                {
                    continue;
                }
                string[] l = p[i].Split(new string[1] { sep_2 }, StringSplitOptions.None);
                if(l.Length < 1)
                {
                    continue;
                }
                string[] bc = l[0].Split('.');
                if(bc.Length < 2)
                {
                    Debug.LogWarning("Error parsing entity string");
                    continue;
                }
                IBaseComponent b = e.GetComponent(bc[0]);
                if(b == default)
                {
                    Debug.LogWarning($"Could not find {bc[0]} for {e.entity_string_id}");
                    s += l[0] + l[1];
                    continue;
                }
                FieldInfo f = b.GetType().GetField(bc[1]);
                if(f == null)
                {
                    Debug.LogWarning($"Could not find field {bc[1]} for {bc[0]} when parsing using {e.entity_string_id}");
                    s += l[0] + l[1];
                    continue;
                }
                s += f.GetValue(b).ToString();
                s += l[1];
            }

            return s;
        }
    }
}
