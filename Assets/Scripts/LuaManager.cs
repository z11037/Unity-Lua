using UnityEngine;
using XLua;
using System.IO; // 新增：用于读取文件

public class LuaManager
{
    public static LuaManager Instance;
    private LuaEnv luaEnv;

    public void Init()
    {
        Instance = this;
        luaEnv = new LuaEnv();

        // ========== 新增：配置xLua自定义加载器（核心！） ==========
        luaEnv.AddLoader(CustomLuaLoader);
    }

    // 自定义加载器：根据模块名，加载Assets下的Lua文件
    private byte[] CustomLuaLoader(ref string moduleName)
    {
        // 1. 把模块名（如luaScript/1）转为Assets下的文件路径
        // 替换路径分隔符（兼容Windows/Mac），拼接成完整路径
        string luaPath = $"{Application.dataPath}/{moduleName}.lua";
        luaPath = luaPath.Replace('/', Path.DirectorySeparatorChar);

        // 2. 检查文件是否存在，存在则读取为字节数组返回（xLua需要字节数组）
        if (File.Exists(luaPath))
        {
            try
            {
                // 读取文件，编码用UTF-8（避免中文乱码）
                byte[] luaBytes = File.ReadAllBytes(luaPath);
                Debug.Log($"成功加载Lua文件：{luaPath}");
                return luaBytes;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"读取Lua文件失败：{e.Message}");
                return null;
            }
        }
        else
        {
            Debug.LogError($"Lua文件不存在：{luaPath}");
            return null;
        }
    }

    public object[] DoString(string lua)
    {
        // 加空值检查，避免luaEnv未初始化崩溃
        if (luaEnv == null)
        {
            Debug.LogError("LuaEnv未初始化！");
            return null;
        }
        return luaEnv.DoString(lua);
    }

    public void Tick()
    {
        if (luaEnv != null)
            luaEnv.Tick();
    }

    // 新增：释放LuaEnv，避免内存泄漏
    public void Dispose()
    {
        if (luaEnv != null)
        {
            luaEnv.Dispose();
            luaEnv = null;
        }
    }
    public void Reload(string luaName)
    {
        luaEnv.DoString($"package.loaded['{luaName}'] = nil");
    }
}