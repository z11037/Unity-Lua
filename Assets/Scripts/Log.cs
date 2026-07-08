using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Log
{
    public static void Skill(string msg)
    {
        Debug.Log($"<color=#00FFFF>[Skill]</color> {msg}");
    }

    public static void Buff(string msg)
    {
        Debug.Log($"<color=#FFFFFF>[Buff]</color> {msg}");
    }
}
