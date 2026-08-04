using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public static class SkillValidator
{
    public static List<string> Validate(List<SkillSO> skills)
    {
        var errors = new List<string>();

        foreach (var skill in skills)
        {
            if (string.IsNullOrWhiteSpace(skill.skillName))
                errors.Add($"警告：技能 ID {skill.skillID} 名称不能为空。");
            if (skill.cooldown < 0)
                errors.Add($"警告：技能 {skill.skillName} 冷却时间不能为负数。");
        }

        var idSet = new HashSet<int>();
        foreach (var skill in skills)
        {
            if (!idSet.Add(skill.skillID))
                errors.Add($"警告：技能 {skill.skillName} 的 ID {skill.skillID} 重复。");
        }

        var nameMap = new Dictionary<string, int>();
        foreach (var skill in skills)
        {
            if (string.IsNullOrWhiteSpace(skill.skillName)) continue;
            if (nameMap.ContainsKey(skill.skillName))
            {
                if (nameMap[skill.skillName] != skill.skillID)
                    errors.Add($"警告：技能名称 \"{skill.skillName}\" 重复。");
            }
            else
                nameMap.Add(skill.skillName, skill.skillID);
        }
        foreach (var skill in skills)
        {

            if (string.IsNullOrWhiteSpace(skill.filePath))
            {
                Debug.LogWarning(
                    $"警告：技能 {skill.skillName} 的 Lua 引用丢失");
            }
        }

        return errors;
    }
}