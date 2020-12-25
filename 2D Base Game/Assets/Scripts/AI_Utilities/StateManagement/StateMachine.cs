using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Psingine
{
    public class StateMap
    {
        List<Path_Node<IGoal>> state_map;

    }

    public class EntityStateEdge : IRequirement<Entity>
    {
        public EntityStateMachine state;
        EntityAction on_transition;
        public List<IRequirement<Entity>> reqs;

        public EntityStateEdge(EntityStateMachine machine)
        {
            state = machine;
            reqs = new List<IRequirement<Entity>>();
            on_transition = new EntityAction(machine.state_id);
        }

        public EntityStateEdge(EntityStateEdge edge)
        {
            state = edge.state;
            on_transition = new EntityAction(edge.on_transition);
            reqs = new List<IRequirement<Entity>>(reqs);
        }

        public bool Satisfies(Entity t)
        {
            foreach(IRequirement<Entity> r in reqs)
            {
                if (!r.Satisfies(t))
                {
                    return false;
                }
            }
            return true;
        }

        public int Preference(Entity e)
        {
            return 1;
        }

        public EntityStateEdge SetEdge(Entity source, Entity target)
        {
            on_transition.Invoke(source, target);
            return this;
        }
    }

    public class EntityStateMachine
    {
        public string state_id;
        EntityStateEdge back_edge;
        List<EntityStateEdge> available_paths;
        EntityAction call_action;
        Entity entity;

        public EntityStateMachine(string state_id)
        {
            this.state_id = state_id;
            available_paths = new List<EntityStateEdge>();
            back_edge = new EntityStateEdge(this);
        }

        /// <summary>
        /// Constructor for copying a state machine
        /// </summary>
        /// <param name="state"></param>
        public EntityStateMachine(EntityStateMachine state)
        {
            state_id = state.state_id;
            available_paths = new List<EntityStateEdge>(state.available_paths);
            back_edge = new EntityStateEdge(state);
            call_action = new EntityAction(state.call_action);
            entity = state.entity;
        }

        public EntityStateMachine(Entity e, EntityStateMachine state)
        {
            state_id = state.state_id;
            available_paths = new List<EntityStateEdge>(state.available_paths);
            back_edge = state.back_edge;
            call_action = new EntityAction(state.call_action);
            entity = e;
        }

        public EntityStateMachine Transition(Entity entity, Entity target)
        {
            List<EntityStateEdge> edges = new List<EntityStateEdge>();
            int max_pref = 0;

            foreach(EntityStateEdge e in available_paths)
            {
                if(e.Satisfies(entity) && e.Preference(entity) >= max_pref)
                {
                    if(e.Preference(entity) == max_pref)
                    {
                        edges.Add(e);
                    } else
                    {
                        edges = new List<EntityStateEdge> { e };
                        max_pref = e.Preference(entity);
                    }
                }
            }

            //Returns the state with the highest preference, or the default state if there are no max pref states
            //Default transition should be to just repeat whatever is happening
            return edges.Count > 0 ? new EntityStateMachine(entity, edges[Random.Range(0,edges.Count-1)].SetEdge(entity, target).state) : back_edge.SetEdge(entity, target).state;
        }
    }
}

