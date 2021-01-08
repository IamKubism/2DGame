using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class ItemVector<T1, T2>
    {
        public T1 a;
        public T2 b;

        public ItemVector(T1 a, T2 b)
        {
            this.a = a;
            this.b = b;
        }
    }

    public class ItemVector<T1, T2, T3>
    {
        public T1 a;
        public T2 b;
        public T3 c;

        public ItemVector(T1 a, T2 b, T3 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }
    }

}
