using MoonSharp.Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonsharpGenericTypeRegister<T>
{
    public void RegisterType()
    {
        UserData.RegisterType<T>();
    }

    public void RegisterObject(T t)
    {

    }
}
