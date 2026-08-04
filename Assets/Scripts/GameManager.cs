using UnityEngine;

public class GameManager : MonoBehaviour
{
    private LuaManager luaManager;

    private void Awake()
    {
        luaManager = new LuaManager();
        luaManager.Init();

        Debug.Log("GameManager初始化完成");
    }

    private void Update()
    {
        if (luaManager != null)
        {
            luaManager.Tick();
        }
    }

    private void OnDestroy()
    {
        if (luaManager != null)
        {
            luaManager.Dispose();
            luaManager = null;
        }

        Debug.Log("GameManager已销毁");
    }
}