using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public class MovementCalculator : IBehavior
    {
        public Dictionary<string, Func<Entity, float>> id_to_computation;

        public MovementCalculator()
        {
            id_to_computation = new Dictionary<string, Func<Entity, float>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m1"></param>
        public MovementCalculator(MovementCalculator m1)
        {
            id_to_computation = new Dictionary<string, Func<Entity, float>>(m1.id_to_computation);
        }

        public float CalculateOnEntity(Entity e)
        {
            float sum = 0;

            foreach(Func<Entity,float> f in id_to_computation.Values)
            {
                sum = Sum(sum, f(e));
            }

            return sum;
        }

        public float Sum(float f1, float f2)
        {
            return Mathf.Min(Mathf.Sign(f1), Mathf.Sign(f2)) * (Mathf.Abs(f1) + Mathf.Abs(f2));
        }

        /// <summary>
        /// TODO: this is kinda dumb
        /// </summary>
        /// <typeparam name="CompType"></typeparam>
        /// <param name="float_computer"></param>
        /// <param name="comp_name"></param>
        public void SetComputation<CompType>(Func<CompType,float> float_computer, string component_name, string computation_name)
        {
            if (id_to_computation.ContainsKey(computation_name))
            {
                id_to_computation[computation_name] = (ent) => {
                    if (ent.GetComponent<CompType>(component_name) == default)
                    {
                        return 0.0000000001f;
                    }
                    return float_computer(ent.GetComponent<CompType>(component_name));
                };
            } else
            {
                id_to_computation.Add(component_name, (ent) => {
                    if (ent.GetComponent<CompType>(component_name) == default)
                    {
                        return 0.0000000001f;
                    }
                    return float_computer(ent.GetComponent<CompType>(component_name));
                });
            }
        }

        public void RemoveComputation(string computation_name)
        {
            if (id_to_computation.ContainsKey(computation_name))
            {
                id_to_computation.Remove(computation_name);
            }
        }

        public static MovementCalculator test_calculator;

        public static void SetTestCalculator()
        {
            test_calculator = new MovementCalculator();
            test_calculator.SetComputation<Terrain>(
            (t) =>
            {
                return t.move_cost;
            }, "Terrain", "TerrainCost");
        }
    }
}
