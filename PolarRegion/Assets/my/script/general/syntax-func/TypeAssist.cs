using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeAssist
{
    public static bool WhetherTypeIs<B>(object target) where B:class
    {
        if (target as B == null) return false;
        else return true;
    }
}
