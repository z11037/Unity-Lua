using UnityEditor;
using XLua;

[InitializeOnLoad]
public static class XLuaConfig
{
    static XLuaConfig()
    {
        // 启用 XLua 热更特性，解决编译兼容问题
        DefineSymbols.Add("XLUA_ENABLE");
    }
}

public static class DefineSymbols
{
    // 向 Unity 添加编译宏
    public static void Add(string symbol)
    {
        BuildTargetGroup buildGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildGroup);
        if (!symbols.Contains(symbol))
        {
            symbols += $";{symbol}";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, symbols);
        }
    }
}