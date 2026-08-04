using System;
using System.IO;
using UnityEngine;
using XLua;

public class LuaManager
{
    public static LuaManager Instance { get; private set; }

    private LuaEnv luaEnv;

    public void Init()
    {
        if (luaEnv != null)
        {
            Debug.LogWarning("LuaManager已经初始化");
            return;
        }

        Instance = this;
        luaEnv = new LuaEnv();
        luaEnv.AddLoader(CustomLuaLoader);

        Debug.Log("LuaManager初始化完成");
    }

    public string NormalizeLuaModuleName(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return string.Empty;
        }

        string moduleName = filePath.Trim().Replace('\\', '/');

        if (moduleName.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
        {
            moduleName = moduleName.Substring("Assets/".Length);
        }

        if (moduleName.EndsWith(".lua", StringComparison.OrdinalIgnoreCase))
        {
            moduleName = moduleName.Substring(0, moduleName.Length - ".lua".Length);
        }

        return moduleName;
    }

    private byte[] CustomLuaLoader(ref string moduleName)
    {
        string normalizedModuleName = NormalizeLuaModuleName(moduleName);

        if (string.IsNullOrEmpty(normalizedModuleName))
        {
            Debug.LogError("Lua模块名为空");
            return null;
        }

        moduleName = normalizedModuleName;

        string relativePath = normalizedModuleName + ".lua";
        relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);

        string fullPath = Path.Combine(Application.dataPath, relativePath);

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"Lua文件不存在：{fullPath}");
            return null;
        }

        try
        {
            byte[] luaBytes = File.ReadAllBytes(fullPath);
            Debug.Log($"成功加载Lua文件：{fullPath}");
            return luaBytes;
        }
        catch (Exception exception)
        {
            Debug.LogError($"读取Lua文件失败：{fullPath}");
            Debug.LogException(exception);
            return null;
        }
    }

    public LuaTable RequireModule(string filePath)
    {
        if (luaEnv == null)
        {
            Debug.LogError("LuaEnv未初始化");
            return null;
        }

        string moduleName = NormalizeLuaModuleName(filePath);

        if (string.IsNullOrEmpty(moduleName))
        {
            Debug.LogError("无法加载Lua模块：文件路径为空");
            return null;
        }

        string escapedModuleName = EscapeLuaString(moduleName);
        object[] results = luaEnv.DoString($"return require('{escapedModuleName}')");

        if (results == null || results.Length == 0)
        {
            Debug.LogError($"Lua模块没有返回结果：{moduleName}");
            return null;
        }

        LuaTable table = results[0] as LuaTable;

        if (table == null)
        {
            Debug.LogError($"Lua模块返回值不是LuaTable：{moduleName}");
            return null;
        }

        return table;
    }

    public object[] DoString(string lua)
    {
        if (luaEnv == null)
        {
            Debug.LogError("LuaEnv未初始化");
            return null;
        }

        return luaEnv.DoString(lua);
    }

    public void Tick()
    {
        if (luaEnv != null)
        {
            luaEnv.Tick();
        }
    }

    public void Reload(string filePath)
    {
        if (luaEnv == null)
        {
            Debug.LogError("LuaEnv未初始化，无法重载Lua模块");
            return;
        }

        string moduleName = NormalizeLuaModuleName(filePath);

        if (string.IsNullOrEmpty(moduleName))
        {
            Debug.LogError("无法重载Lua模块：文件路径为空");
            return;
        }

        string escapedModuleName = EscapeLuaString(moduleName);
        luaEnv.DoString($"package.loaded['{escapedModuleName}'] = nil");

        Debug.Log($"已清除Lua模块缓存：{moduleName}");
    }

    public void Dispose()
    {
        if (luaEnv != null)
        {
            luaEnv.Dispose();
            luaEnv = null;
        }

        if (Instance == this)
        {
            Instance = null;
        }

        Debug.Log("LuaManager已释放");
    }

    private string EscapeLuaString(string value)
    {
        return value.Replace("\\", "\\\\").Replace("'", "\\'");
    }
}