using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


///
/// This is basically me trying to generalize the "mover" protocol, so that similar systems can be defined easily from external sources
///




/// <summary>
/// A struct that defines an update on an entity for a function that requires the argument type T
/// </summary>
/// <typeparam name="T"></typeparam>
public struct Updater<T>
{
    private readonly string entity_string_id;
    private readonly T argument;


    public Updater(string enity_id, T argument): this()
    {
        entity_string_id = enity_id;
        this.argument = argument;
    }


    public T argument_visible
    {
        get
        {
            return argument;
        }
    }


    public string entity_id
    {
        get
        {
            return entity_string_id;
        }
    }
}



/// <summary>
/// An updater that is run continuously and does not have return values that it uses.
/// </summary>
/// <typeparam name="T"></typeparam>
public class VoidUpdaterList<T>
{
    List<string> entities_to_update;
    List<T> arguments;
    Action<VoidUpdaterList<T> ,string, T> update;

    public VoidUpdaterList(Action<VoidUpdaterList<T>, string,T> update)
    {
        entities_to_update = new List<string>();
        arguments = new List<T>();
        this.update = update;
    }

    public void Update()
    {
        for (int i = entities_to_update.Count; i > 0; i -= 1)
        {
            update(this, entities_to_update[i - 1], arguments[i - 1]);
        }
    }

    /// <summary>
    /// Used to both add an entity to the update queue and also to change the argument value of the update for the entity if already queued
    /// </summary>
    /// <param name="entity_id"></param>
    /// <param name="arg"></param>
    public void AddEntityToUpdate(string entity_id, T arg)
    {
        if (entities_to_update.Contains(entity_id))
        {
            arguments[entities_to_update.IndexOf(entity_id)] = arg;
        } else
        {
            entities_to_update.Add(entity_id);
            arguments.Add(arg);
        }
    }
}

/* Examples of the update functions (pseudocode)
 * 
 *  UpdateTileType(tile_id, type_id) :
 *      SetTileType(tile_id, type_id)
 *      Print("Tile type changed %tile_id to %type_id") 
 *      j = IndexOf(tile_id)
 *      entities_to_update.removeat(j)
 *      args.removeat(j)
 */
