using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Psingine;

public interface ISystemAdder
{
    void AddEntities(List<Entity> entities);
    void AddEntity(Entity e);
    string SysCompName();
}