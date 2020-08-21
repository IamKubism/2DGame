using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class Path_Edge<T>
    {
        public float modifier;
        public Path_Node<T> node;

        public Path_Edge(Path_Node<T> node, float modifier = 1f)
        {
            this.node = node;
            this.modifier = modifier;
        }
    }
}
