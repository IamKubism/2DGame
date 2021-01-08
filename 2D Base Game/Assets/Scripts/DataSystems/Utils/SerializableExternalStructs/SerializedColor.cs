using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class SerializedColor
    {
        string s_color;
        float[] rgba;

        public SerializedColor()
        {
            rgba = new float[4] { 0, 0, 0, 1 };
        }

        public SerializedColor(float r, float g, float b, float a)
        {
            rgba = new float[4] { r, g, b, a };
            s_color = $"{r}:{g}:{b}:{a}";
        }

        /// <summary>
        /// Parses something like 0.5:0.0:1:1
        /// </summary>
        /// <param name="s"></param>
        public SerializedColor(string s)
        {
            rgba = new float[4] { 0, 0, 0, 1 };
            string[] set_vals = new string[4] { "0", "0", "0", "1" };
            string[] vals = s.Split(':');
            int i = 0;
            while (i < vals.Length)
            {
                set_vals[i] = vals[i];
                rgba[i] = float.Parse(vals[i]);
                i += 1;
            }
            s = $"{set_vals[0]}:{set_vals[1]}:{set_vals[2]}:{set_vals[3]}";
        }

        public static implicit operator Color(SerializedColor sc)
        {
            return new Color(sc.rgba[0], sc.rgba[1], sc.rgba[2], sc.rgba[3]);
        }
    }
}


