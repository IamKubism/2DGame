using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class Dice
    {
        int c;
        int n;
        int d;

        public Dice()
        {
            c = 0;
            n = 0;
            d = 1;
        }

        public Dice(int c, int d)
        {
            this.c = c;
            this.n = 1;
            this.d = d;
        }

        public Dice(string s)
        {
            string[] parsed = s.Split('d');
            b = int.Parse(parsed[0]);
            d = new List<int> { int.Parse(parsed[1]) };
        }

        public override string ToString()
        {
            string s = b.ToString() + "d" + d[0];
            for(int i = 1; i < d.Count; i += 1)
            {
                s += "," + d[i].ToString();
            }

            return s;
        }

        public static Dice operator +(Dice d1, Dice d2)
        {
            List<int> d = new List<int>(d1.d);
            d.AddRange(d2.d);
            return new Dice(d1.b + d2.b, d);
        }

        public static int Roll(Dice d)
        {
            int total = d.b;
            foreach(int i in d.d)
            {
                total += Random.Range(0, i);
            }
            return total;
        }
    }
}

