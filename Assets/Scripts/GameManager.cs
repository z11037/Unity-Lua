using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        LuaManager.Instance = new LuaManager();
        LuaManager.Instance.Init();
    }

}
