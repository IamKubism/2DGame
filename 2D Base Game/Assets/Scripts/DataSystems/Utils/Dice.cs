using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Psingine
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

        public Dice(int c, int n, int d)
        {
            this.c = c;
            this.n = n;
            this.d = d;
        }

        public Dice(string s)
        {
            c = n = 0; d = 1;
            string[] parsed = s.Split('+','d');
            int i = 0;
            if (s.Contains("+"))
            {
                if(parsed[i] != "")
                    c = int.Parse(parsed[i]);
                i += 1;
            }
            if (s.Contains("d"))
            {
                if (parsed[i] != "")
                    n = int.Parse(parsed[i]);
                if (parsed[i + 1] != "")
                    d = int.Parse(parsed[i + 1]);
            }
        }

        public Dice(Dice dice)
        {
            c = dice.c;
            n = dice.n;
            d = dice.d;
        }

        public void SetNum(int n)
        {
            this.n = n;
        }

        public void SetSize(int d)
        {
            this.d = d;
        }

        public void SetConst(int c)
        {
            this.c = c;
        }

        public void AddSize(int dd)
        {
            d = Mathf.Max(1, d + dd);
        }

        public void AddDiceNum(int dn)
        {
            n = Mathf.Max(0, n+dn);
        }

        public void AddConst(int dc)
        {
            c += dc;
        }

        public override string ToString()
        {
            return $"{c}+{n}d{d}";
        }

        public static implicit operator int(Dice d)
        {
            return Roll(d);
        }

        public static List<Dice> operator +(Dice d1, Dice d2)
        {
            return new List<Dice> { d1, d2 };
        }

        public static List<Dice> operator +(List<Dice> ds, Dice d)
        {
            ds.Add(d);
            return ds;
        }

        public static Dice operator *(Dice dice, int mod)
        {
            Dice d2 = new Dice(dice);
            d2.d *= mod;
            d2.c *= mod;
            return dice;
        }

        public static implicit operator DiceGroup(Dice d)
        {
            return new DiceGroup(d);
        }

        public static implicit operator string(Dice d)
        {
            return d.ToString();
        }

        public static implicit operator Dice(string s)
        {
            return new Dice(s);
        }

        public static List<Dice> ParseDice(string s)
        {
            string[] dice = s.Split(',');
            List<Dice> d = new List<Dice>();
            foreach (string ds in dice)
            {
                if(ds != "")
                    d.Add(new Dice(ds));
            }
            return d;
        }

        public static int Roll(Dice d)
        {
            int total = d.c;
            for(int i = 0; i < d.n; i += 1)
            {
                total += Random.Range(0, d.d);
            }
            return Mathf.Max(0,total);
        }

        public static int RollAll(List<Dice> ds)
        {
            int total = 0;
            foreach(Dice d in ds)
            {
                total += d;
            }
            return total;
        }


    }

    public class DiceGroup
    {
        int displacement;
        List<Dice> group;

        public DiceGroup()
        {
            displacement = 0;
            group = new List<Dice>();
        }

        public DiceGroup(Dice d)
        {
            group = new List<Dice> { d };
            displacement = 0;
        }

        public DiceGroup(int i)
        {
            displacement = i;
            group = new List<Dice>();
        }

        public DiceGroup(string s)
        {
            group = new List<Dice>();
            displacement = 0;
            string[] vals = s.Split(':');
            int i = 0;
            if (vals.Length > 1 && vals[i] != "")
            {
                displacement = int.Parse(vals[i]);
                i += 1;
            }
            if (vals[i] != "")
                group = Dice.ParseDice(vals[i]);
        }

        public DiceGroup(DiceGroup g)
        {
            group = new List<Dice>();
            foreach(Dice d in g.group)
            {
                group.Add(new Dice(d));
            }
            displacement = g.displacement;
        }

        public void AddToDisplacement(int disp)
        {
            displacement += disp;
        }

        public override string ToString()
        {
            string s = $"{displacement}:";
            if(group.Count == 0)
            {
                s += "0d1";
                return s;
            }
            s += $"{group[0].ToString()}";
            int i = 1;
            while(i < group.Count)
            {
                s += $",{group[i].ToString()}";
            }
            return s;
        }

        public static DiceGroup operator +(DiceGroup d1, DiceGroup d2)
        {
            DiceGroup c = new DiceGroup(d1);
            foreach(Dice d in d2.group)
            {
                c.group.Add(new Dice(d));
            }
            return c;
        }

        public static DiceGroup operator +(DiceGroup d1, int disp)
        {
            DiceGroup c = new DiceGroup(d1);
            c.displacement += disp;
            return c;
        }

        public static DiceGroup operator *(DiceGroup d1, int mod)
        {
            DiceGroup c = new DiceGroup(d1);
            for(int i = 0; i < c.group.Count; i += 1)
            {
                c.group[i] *= mod;
            }
            return c;
        }
        
        public static implicit operator DiceGroup(string s)
        {
            return new DiceGroup(s);
        }

        public static implicit operator string(DiceGroup d)
        {
            return d.ToString();
        }

        public static implicit operator int(DiceGroup dg)
        {
            return Mathf.Max(0,Dice.RollAll(dg.group)+dg.displacement);
        }
    }
}

