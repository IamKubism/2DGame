using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Struct that gives a set of floats that trigger things on reaching a minimum or maximum
/// </summary>
public class FloatMinMax
{
    /// <summary>
    /// Minimum value of curr, for the "under_min" trigger 
    /// </summary>
    public float min;

    /// <summary>
    /// Current value for this component
    /// </summary>
    public float curr;

    /// <summary>
    /// Maximum value of curr for the "over_max" trigger
    /// </summary>
    public float max;

    /// <summary>
    /// Multiplicative modifier for the "dt" updater method
    /// </summary>
    public float dt_mod;

    public FloatMinMax(float curr = 0f, float max = 1f, float min = 0f, float dt_mod = 1f)
    {
        this.curr = curr;
        this.max = max;
        this.min = min;
        this.dt_mod = dt_mod;
    }

    public FloatMinMax(FloatMinMax fl)
    {
        curr = fl.curr;
        max = fl.max;
        min = fl.min;
        dt_mod = fl.dt_mod;
    }

    public bool IsOverMax()
    {
        return curr >= max;
    }

    public bool IsUnderMin()
    {
        return curr <= min;
    }

    public float NormalizedByMax()
    {
        return curr / max;
    }

    /// <summary>
    /// Reseter for the float's dt_mod
    /// </summary>
    /// <param name="x"></param>
    /// <param name="dt_mod"></param>
    /// <returns></returns>
    public static FloatMinMax operator *(FloatMinMax x, float dt_mod)
    {
        x.dt_mod = dt_mod;
        return x;
    }

    /// <summary>
    /// Advance the component's curr
    /// </summary>
    /// <param name="x"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public static FloatMinMax operator +(FloatMinMax x, float d)
    {
        x.curr += d * x.dt_mod;
        return x;
    }

    public void Reset(float max = 1f, float min = 0f)
    {
        this.max = max;
        this.min = min;
        curr = 0f;
        //Debug.Log("curr = " + curr);
    }
}