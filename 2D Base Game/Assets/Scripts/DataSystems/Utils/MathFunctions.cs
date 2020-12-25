using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Psingine
{

    public class MathFunctions
    {
        public static MathFunctions inst;

        public MathFunctions()
        {
            if (inst == null)
            {
                inst = this;
            }
        }

        /// <summary>
        /// Returns i^exp
        /// </summary>
        /// <param name="i"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static int Pow(int i, int exp)
        {
            byte x = (byte)exp;
            int l = 1;
            int y = 1;
            while (x > 0)
            {
                l *= i;
                if ((x & 1) == 1)
                {
                    y *= l;
                }
                x >>= 1;
            }
            return y;
        }

        public static int Fact(int i)
        {
            int x = 1;
            int y = i;
            while (y > 0)
            {
                x *= y;
                y -= 1;
            }
            return x;
        }

        public static float ManhattonDist(float[] p1, float[] p2)
        {
            float dist = 0;

            for (int i = 0; i < p1.Length; i += 1)
            {
                if (Mathf.Abs(p1[i] - p2[i]) > dist)
                {
                    dist = Mathf.Abs(p1[i] - p2[i]);
                }
            }

            return dist;
        }

        public static int SqrDist(int[] p1, int[] p2)
        {
            if (p1.Length != p2.Length)
            {
                Debug.LogError("Cannot calculate distance between two different dimension points");
            }
            int dist = 0;
            for (int i = 0; i < p1.Length; i += 1)
            {
                int displacement = (p1[i] - p2[i]);
                dist += displacement*displacement;
            }

            return dist;
        }

        public static float Dot(float[] p1, float[] p2)
        {
            float total = 0;
            for(int i = 0; i < Mathf.Min(p1.Length, p2.Length); i += 1)
            {
                total += p1[i] * p2[i];
            }
            return total;
        }

        public static int SqrDist(Position.Tile p1, Position.Tile p2)
        {
            return Pow(p1.x - p2.x,2)+Pow(p1.y-p2.y,2) + Pow(p1.z-p2.z,2);
        }

        public static Vector3 PositionDirection(Position p1, Position p2)
        {
            return (p2.t_r-p1.t_r).normalized;
        }

        public static float NegativeRespectingSum(float f1, float f2)
        {
            return Mathf.Min(Mathf.Sign(f1), Mathf.Sign(f2))*(Mathf.Abs(f1)+Mathf.Abs(f2));
        }

        public static int NegativeRespectingSum(int f1, int f2)
        {
            return (int)(Mathf.Min(Mathf.Sign(f1), Mathf.Sign(f2)))*(Mathf.Abs(f1)+Mathf.Abs(f2));
        }
    }
}
