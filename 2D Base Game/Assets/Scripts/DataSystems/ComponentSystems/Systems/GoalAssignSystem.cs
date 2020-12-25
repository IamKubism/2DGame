using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Psingine
{
    public enum AIEvents { EvalGoal, GetGoals, SetCurrGoalData, SetNewGoal, GetCurrGoalData, GetPref }

    public enum AIParams
    {
        goal_priorities,
        next_goal,
        random_select,
        goal_complete,
        priority,
        goal_target,
        sub_goals
    }

    public class GoalAssignSystem
    {
        public static GoalAssignSystem instance;

        public GoalAssignSystem()
        {
            if(instance == null)
            {
                instance = this;
            }
            EventManager.instance.GetPrototype(AIEvents.GetGoals.ToString()).SetParamValue(AIParams.goal_priorities, new List<(Entity, float)>(), (l1, l2) => { return l1; });
            EventManager.instance.GetPrototype(AIEvents.GetGoals.ToString()).SetParamValue(AIParams.sub_goals, new List<Entity>(), (l1, l2) => { return l1; });
            EventManager.instance.GetPrototype(AIEvents.GetGoals.ToString()).AddUpdate(SetGoalPrefs, 480);
        }

        public bool AISetLoop(Event e)
        {
            Entity invoker = e.GetParamValue<Entity>(EventParams.invoking_entity);
            Conscious conscious = invoker.GetComponent<Conscious>();

            //TODO: What should a goal be represented by? (currently thinking an entity cause I can probably just give any goal information as components)
            Entity curr_goal = conscious.curr_goal;
            Event get_goals = Event.NewEvent(AIEvents.GetCurrGoalData);
            get_goals.Invoke(curr_goal);

            //Alright this is the part that is currently not super defined
            get_goals = Event.NewEvent(get_goals, AIEvents.GetGoals);
            get_goals.Invoke(invoker);
            List<(Entity, float)> goal_priorities = get_goals.GetParamValue<List<(Entity,float)>>(AIParams.goal_priorities);

            float total_priority = 0;

            foreach ((Entity,float) si in goal_priorities)
            {
                if(si.Item2 > 0)
                    total_priority += si.Item2;
            }
            bool eval = false;
            if(total_priority <= 0)
            {
                return false;
            }

            bool rand = false;
            if(get_goals.HasParamValue(AIParams.random_select))
            {
                rand = get_goals.GetParamValue<bool>(AIParams.random_select);
            }

            if (rand)
            {
                float pref_num = Random.Range(0, total_priority);
                total_priority = 0;

                foreach ((Entity, float) si in goal_priorities)
                {
                    if (si.Item2 > 0)
                    {
                        total_priority += si.Item2;
                        if (pref_num <= total_priority)
                        {
                            //TODO: Set next goal
                            Event set_goal = Event.NewEvent(AIEvents.SetNewGoal);
                            set_goal.SetParamValue(AIParams.next_goal, si.Item1, (e1, e2) => { return e2; });
                            set_goal.Invoke(invoker);
                            eval = true;
                            break;
                        }
                    }
                }
            } else
            {
                Entity top_goal = null;
                total_priority = 0;
                foreach((Entity, float) si in goal_priorities)
                {
                    if(si.Item2 > total_priority)
                    {
                        total_priority = si.Item2;
                        top_goal = si.Item1;
                    }
                }
                if(top_goal != null)
                {
                    Event set_goal = Event.NewEvent(AIEvents.SetNewGoal);
                    set_goal.SetParamValue(AIParams.next_goal, top_goal, (e1, e2) => { return e2; });
                    set_goal.Invoke(invoker);
                    eval = true;
                }
            }

            return eval;
        }

        public void AIEvalLoop(Event e, Entity ent)
        {
            //TODO: Something like this but idk about what should trigger it
            if(ent.GetComponent<Conscious>().CheckGoalsForward(true))
            {
                Event goal_set = Event.NewEvent(e, AIEvents.SetCurrGoalData);
                goal_set.Invoke(ent);
                while (AISetLoop(goal_set))
                {
                }
            }
        }

        public void SetGoalPrefs(Event e)
        {
            List<(Entity, float)> prefs = new List<(Entity, float)>();
            foreach(Entity goal in e.GetParamValue<List<Entity>>(AIParams.sub_goals))
            {
                Event compute_pref = Event.NewEvent(e, AIEvents.GetPref);
                compute_pref.Invoke(goal);
                prefs.Add((goal, compute_pref.GetParamValue<float>(AIParams.priority)));
            }
            e.SetParamValue(AIParams.goal_priorities, prefs, (p1, p2) => { p1.AddRange(p2); return p1; });
        }
    }

}


/* PLANNING
 * State based AI (Lack of explicit entity planning)
 * Specific Routine: Deal with Hostile (Attacks)
 *      -> GetPrefAttack
 *          -> CreateAINode : Creates node entity with the goal's relevant info, and then adds this node to the conscious
 *              - Get Sub Nodes from attack and entity components
 *              Ex: { AddAttackToTurnQueue, MoveToPrefArea }
 *              -> SetEventToCall
 *                  -> CreateNextNode
 *                  ...
 *                      -> NoTransitions : End
 *  Creating sub nodes:
 *      Current node has a list of things that must be achieved to complete.
 *      - Complete/ execute task is a node, activated on turn end and always creates a new node with no sub nodes (maybe always)
 *      
 */