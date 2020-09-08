using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighKings;

public interface ISystemAdder
{
    void AddEntities(List<Entity> entities);
    string SysCompName();
}