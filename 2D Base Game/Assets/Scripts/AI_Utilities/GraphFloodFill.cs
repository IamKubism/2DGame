using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Psingine
{
    public class GraphFloodFill
    {
        public List<Entity> area;

        public GraphFloodFill(Dictionary<Entity,Path_Node<Entity>> graph, Event cost_event, Entity source)
        {
            area = new List<Entity>();

            int Cost(Entity curr)
            {
                Event e2 = new Event(cost_event);
                e2.Alter(curr);
                e2.Invoke(curr);

                return e2.GetParamValue<int>("flood_cost");
            }

            List<Path_Node<Entity>> closed_set = new List<Path_Node<Entity>>();
            Queue<Path_Node<Entity>> open_set = new Queue<Path_Node<Entity>>();
            open_set.Enqueue(graph[source]);

            while(open_set.Count > 0)
            {
                Path_Node<Entity> current = open_set.Dequeue();
                closed_set.Add(current);
                if(Cost(current.data) > 0)
                {
                    area.Add(current.data);
                    foreach(Path_Edge<Entity> ed in current.edges)
                    {
                        if(!(open_set.Contains(ed.node) || closed_set.Contains(ed.node)))
                        {
                            open_set.Enqueue(ed.node);
                        }
                    }
                }
            }
        }

        public GraphFloodFill(Event cost_event, Path_Node<Entity> source)
        {
            area = new List<Entity>();

            int Cost(Entity curr)
            {
                Event e2 = new Event(cost_event);
                e2.Alter(curr);
                e2.Invoke(curr);
                return e2.GetParamValue<int>("flood_cost");
            }

            List<Path_Node<Entity>> closed_set = new List<Path_Node<Entity>>();
            Queue<Path_Node<Entity>> open_set = new Queue<Path_Node<Entity>>();
            open_set.Enqueue(source);

            while(open_set.Count > 0)
            {
                Path_Node<Entity> current = open_set.Dequeue();
                closed_set.Add(current);
                if(Cost(current.data) > 0)
                {
                    area.Add(current.data);
                    foreach(Path_Edge<Entity> ed in current.edges)
                    {
                        if(!(open_set.Contains(ed.node) || closed_set.Contains(ed.node)))
                        {
                            open_set.Enqueue(ed.node);
                        }
                    }
                }
            }
        }

        public void UpdateList(Dictionary<Entity, Path_Node<Entity>> graph, Event cost_event, Entity new_source = null)
        {
            int Cost(Entity curr)
            {
                Event e2 = new Event(cost_event);
                e2.Alter(curr);
                e2.Invoke(curr);
                return e2.GetParamValue<int>("flood_cost");
            }

            List<Path_Node<Entity>> closed_set = new List<Path_Node<Entity>>();
            Queue<Path_Node<Entity>> open_set = new Queue<Path_Node<Entity>>();

            if (new_source != null)
            {
                open_set.Enqueue(graph[new_source]);
            }

            for(int i = area.Count; i > 0; i -= 1)
            {
                Path_Node<Entity> node = graph[area[i - 1]];
                closed_set.Add(node);
                if(Cost(area[i-1]) <= 0)
                {
                    area.RemoveAt(i - 1);
                }
                foreach(Path_Edge<Entity> e in node.edges)
                {
                    if(!(closed_set.Contains(e.node) || open_set.Contains(e.node) || area.Contains(e.node.data)))
                    {
                        open_set.Enqueue(e.node);
                    }
                }
            }

            while (open_set.Count > 0)
            {
                Path_Node<Entity> current = open_set.Dequeue();
                closed_set.Add(current);
                if (Cost(current.data) > 0)
                {
                    area.Add(current.data);
                    foreach (Path_Edge<Entity> ed in current.edges)
                    {
                        if (!(open_set.Contains(ed.node) || closed_set.Contains(ed.node)))
                        {
                            open_set.Enqueue(ed.node);
                        }
                    }
                }
            }
        }
    }
}
