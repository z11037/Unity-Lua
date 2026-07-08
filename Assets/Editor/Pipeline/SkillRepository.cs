using System.Collections.Generic;
using UnityEditor;

public static class SkillRepository
{
    public static List<SkillSO> LoadAll()
    {
        var skills = new List<SkillSO>();

        string[] guids = AssetDatabase.FindAssets(
            "t:SkillSO",
            new[] { SkillPathConfig.SkillFolder });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (path.Contains("/RecycleBin/"))
                continue;

            SkillSO skill =
                AssetDatabase.LoadAssetAtPath<SkillSO>(path);

            if (skill != null)
                skills.Add(skill);
        }


        skills.Sort(
            (a, b) => a.skillID.CompareTo(b.skillID));

        return skills;
    }
}