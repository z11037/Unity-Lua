using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class Test : MonoBehaviour
{
    void Start()
    {
        string luaCode = "require('luaScript/1')";
        LuaManager.Instance.DoString(luaCode);
    }

    void Update()
    {
        if (LuaManager.Instance != null)
            LuaManager.Instance.Tick();
    }

    void OnDestroy()
    {
        if (LuaManager.Instance != null)
            LuaManager.Instance.Dispose();
    }
}
