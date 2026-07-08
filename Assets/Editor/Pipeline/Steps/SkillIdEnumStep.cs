using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class SkillIdEnumStep : IBuildStep
{
    public string StepName => "Generate Skill Enum";

    public BuildResult Execute()
    {
        List<SkillSO> skills = SkillRepository.LoadAll();

        string enumPath = Application.dataPath + "/SkillID.cs";
        using (StreamWriter sw = new StreamWriter(enumPath))
        {
            sw.WriteLine("public enum SkillID");
            sw.WriteLine("{");
            for (int i = 0; i < skills.Count; i++)
            {
                string enumName = ToValidIdentifier(skills[i].skillName, skills[i].skillID);
                string comma = (i == skills.Count - 1) ? "" : ",";
                sw.WriteLine($"    {enumName} = {skills[i].skillID}{comma}");
            }
            sw.WriteLine("}");
        }

        return BuildResult.Ok(skills.Count, $"生成 {skills.Count} 个枚举值");
    }


    private string ToValidIdentifier(string skillName,int skillID)
    {
        if (string.IsNullOrEmpty(skillName))
            return $"Skill_{skillID}";
        string sanitized = Regex.Replace(skillName,@"[^a-zA-Z0-9_\u4e00-\u9fff]","");

        if (string.IsNullOrEmpty(sanitized))
            return $"Skill_{skillID}";

        if (Regex.IsMatch(sanitized, @"[\u4e00-\u9fff]"))
        {
            return $"Skill_{skillID}";
        }

        if (char.IsDigit(sanitized[0]))
            sanitized = "_" + sanitized;

        return sanitized;
    }
}