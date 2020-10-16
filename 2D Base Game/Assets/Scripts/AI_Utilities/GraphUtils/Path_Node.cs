using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class Path_Node<T>
    {
        public T data;

        public List<Path_Edge<T>> edges; // Nodes Leaving out from this node

        public Path_Node(T data)
        {
            edges = new List<Path_Edge<T>>(0x10);
            this.data = data;
        }

        public Path_Edge<T> FindEdge(Path_Node<T> n)
        {
            foreach (Path_Edge<T> e in edges)
            {
                if (e.node == n)
                {
                    return e;
                }
            }
            Debug.LogError("FindEdge tried to locate a missing edge");
            throw new System.Exception("Edge not found exception");
        }

        public bool HasEdge(Path_Node<T> node)
        {
            if (edges == null || edges.Count == 0)
            {
                return false;
            }
            foreach (Path_Edge<T> e in edges)
            {
                if (e.node == node)
                {
                    return true;
                }
            }
            return false;
        }

        public void MakeNewEdge(Path_Node<T> node, float modifier)
        {
            if (HasEdge(node))
            {
                return;
            }
            if (edges.Count == edges.Capacity)
            {
                List<Path_Edge<T>> temp = edges;
                edges = new List<Path_Edge<T>>(edges.Capacity + 0x10);
                edges.AddRange(temp);
            }
            edges.Add(new Path_Edge<T>(node, modifier));
        }

        public void ResetPathEdge(Path_Node<T> node, float modifier)
        {
            if (HasEdge(node))
            {
                Path_Edge<T> edge = FindEdge(node);
                edge.modifier = modifier;
            }
            else
            {
                Debug.LogError("Could not find the correct node");
            }
        }
    }
}

//public class Graph_Node
//{
//    public readonly string node_id;
//    public Graph_Edge[] edges;
//    public NodeEffect node_data;

//    public Graph_Node(string node_id, NodeEffect node_data)
//    {
//        this.node_id = node_id;
//        this.node_data = node_data;
//    }

//    public Graph_Edge FindEdgeForNode(string other_node_id)
//    {
//        foreach(Graph_Edge e in edges)
//        {
//            if (e.to_node == other_node_id)
//            {
//                return e;
//            }
//        }
//        throw new KeyNotFoundException("Edge cannot be found from " + node_id + " to " + other_node_id);
//    }

//    public bool HasEdge(string other_node_id)
//    {
//        foreach (Graph_Edge e in edges)
//        {
//            if (e.to_node == other_node_id)
//            {
//                return true;
//            }
//        }
//        return false;
//    }

//    //Will add and edge between BOTH tiles
//    public void AddEdge(Graph_Node node)
//    {
//        if (node.node_data.IsImpassible())
//        {
//            return;
//        }
//        if (HasEdge(node.node_id) == false)
//        {
//            Graph_Edge[] new_edges = new Graph_Edge[1 + edges.Length];
//            int i = 0;
//            while (i < edges.Length)
//            {
//                new_edges[i] = edges[i];
//                i += 1;
//            }
//            new_edges[i] = new Graph_Edge(i, node.node_data);
//            edges = new_edges;
//        }
//        if (node.HasEdge(node_id))
//        {
//            Graph_Edge[] new_edges_2 = new Graph_Edge[1 + node.edges.Length];
//            int i = 0;
//            while (i < node.edges.Length)
//            {
//                new_edges_2[i] = node.edges[i];
//                i += 1;
//            }
//            new_edges_2[i] = new Graph_Edge(i, node_data);
//            node.edges = new_edges_2;
//        }
//    }

//    public void ChangeData(NodeEffect effect)
//    {
//        node_data = effect;
//    }
//}

//public struct Graph_Edge
//{
//    public readonly int index;
//    public readonly float cost;
//    public string to_node;

//    public Graph_Edge(int edge_id, float cost): this()
//    {
//        this.index = edge_id;
//        this.cost = cost; 
//    }

//    public Graph_Edge(int edge_id, NodeEffect effect) : this()
//    {
//        index = edge_id;
//        cost = effect.base_cost;
//    }
//}