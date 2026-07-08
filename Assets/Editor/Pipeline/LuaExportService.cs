using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
public static class LuaExportService
{

    private static string LuaFolder =>
        Path.Combine(Application.dataPath, "luaScript");
    private static void EnsureLuaFolder()
    {
        if (!Directory.Exists(LuaFolder))
            Directory.CreateDirectory(LuaFolder);
    }
    private static void ExportSkill(SkillSO skill)
    {
        if (string.IsNullOrWhiteSpace(skill.skillName))
            return;

        string luaContent =
$@"-- ====================================
-- Skill : {skill.skillName}
-- ID    : {skill.skillID}
-- Auto Generated
-- ====================================

local skill = {{}}

skill.cooldown = {skill.cooldown}

------------------------------------------------
-- 技能执行入口
------------------------------------------------
function skill.Execute(attacker, target)

    -- TODO 播放动画

    -- TODO 播放特效

    -- TODO 造成伤害
    target:TakeDamage(attacker:GetAttackValue())

end

return skill
";
        string fileName = $"{skill.skillName}_{skill.skillID}.lua";
        string luaPath = Path.Combine(LuaFolder, fileName);
        string relativePath = "Assets/luaScript/" + fileName;

        SerializedObject so = new SerializedObject(skill);
        // 文件已存在则跳过
        if (File.Exists(luaPath))
        {
            so.FindProperty("filePath").stringValue = relativePath;
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(skill);

            return;
        }
        File.WriteAllText(luaPath, luaContent);

       
        so.FindProperty("filePath").stringValue = relativePath;
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(skill);
    }
    public static BuildResult ExportAll()
    {
        List<SkillSO> skills = SkillRepository.LoadAll();

        Export(skills);

        return BuildResult.Ok(
            skills.Count,
            $"导出 {skills.Count} 个 Lua 文件");
    }
    public static void Export(IEnumerable<SkillSO> skills)
    {
        EnsureLuaFolder();

        foreach (var skill in skills)
        {
            ExportSkill(skill);
        }

        AssetDatabase.Refresh();
    }
}