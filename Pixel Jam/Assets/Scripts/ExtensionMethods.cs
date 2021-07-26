using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static Vector2 Flat(this Vector3 v)
    {
        return (Vector2)v;
    }
}
