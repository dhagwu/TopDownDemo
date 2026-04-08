using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public static class XLuaGenConfig
{
    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp = new List<Type>()
    {
        typeof(PlayerAttack),
        typeof(Debug),
    };

    [Hotfix]
    public static List<Type> Hotfix = new List<Type>()
    {
        typeof(PlayerAttack),
    };
}