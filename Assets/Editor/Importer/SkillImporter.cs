using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SkillImporter
{
    [MenuItem("Tools/导入技能CSV")]
    public static void Import()
    {
        // 选择CSV文件
        string path = EditorUtility.OpenFilePanel("选择技能CSV文件", "", "csv");
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("未选择文件，导入取消。");
            return;
        }

        // 检查文件是否存在
        if (!File.Exists(path))
        {
            Debug.LogError($"文件不存在：{path}");
            return;
        }

        // 确保目标文件夹存在
        string folderPath = "Assets/ScriptableObjects/SkillSO";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
                AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "SkillSO");
            AssetDatabase.Refresh();
        }

        //  一次性建立 skillID → SkillSO 的索引（避免循环内反复扫描）
        Dictionary<int, SkillSO> skillMap = new Dictionary<int, SkillSO>();
        string[] allGuids = AssetDatabase.FindAssets("t:SkillSO", new[] { folderPath });
        foreach (string guid in allGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            SkillSO so = AssetDatabase.LoadAssetAtPath<SkillSO>(assetPath);
            if (so != null)
                skillMap[so.skillID] = so;
        }

        //  读取所有行
        string[] lines = File.ReadAllLines(path);
        if (lines.Length < 2)
        {
            Debug.LogWarning("CSV文件至少需要包含表头和一条数据。");
            return;
        }

        int createdCount = 0;
        int updatedCount = 0;
        int failCount = 0;
        List<SkillSO> importedSkills = new List<SkillSO>();
        HashSet<int> importedIds = new HashSet<int>();
        //  逐行解析（跳过表头）
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] cols = line.Split(',');

            // 健壮性检查：列数是否足够
            if (cols.Length < 4)
            {
                Debug.LogWarning($"第{i + 1}行列数不足（需要4列），跳过：{line}");
                failCount++;
                continue;
            }

            // 字段提取与转换
            if (!int.TryParse(cols[0].Trim(), out int id))
            {
                Debug.LogWarning($"第{i + 1}行ID解析失败，跳过：{cols[0]}");
                failCount++;
                continue;
            }

            string name = cols[1].Trim();

            if (!float.TryParse(cols[2].Trim(), out float cd))
            {
                Debug.LogWarning($"第{i + 1}行冷却时间解析失败，跳过：{cols[2]}");
                failCount++;
                continue;
            }

            // 忽略大小写的枚举解析
            if (!Enum.TryParse(cols[3].Trim(), true, out SkillTag tag))
            {
                Debug.LogWarning($"第{i + 1}行Tag解析失败，跳过：{cols[3]}");
                failCount++;
                continue;
            }

            // 检测CSV内重复ID
            if (!importedIds.Add(id))
            {
                Debug.LogWarning($"第{i + 1}行：重复ID {id}，跳过该行。");
                failCount++;
                continue;
            }

            // 从索引中查找或创建 SkillSO
            skillMap.TryGetValue(id, out SkillSO skill);
            bool isNew = (skill == null);

            if (isNew)
            {
                skill = ScriptableObject.CreateInstance<SkillSO>();
                // 文件名使用 ID_Name 格式，避免重名
                string fileName = $"{id}_{name}.asset";
                string assetPath = $"{folderPath}/{fileName}";
                AssetDatabase.CreateAsset(skill, assetPath);
                // 新建后加入索引，防止CSV内重复ID导致重复创建
                skillMap[id] = skill;
                createdCount++;
            }
            else
            {
                updatedCount++;
            }

            // 更新字段（使用 SerializedObject 规范修改）
            SerializedObject so = new SerializedObject(skill);
            so.Update();

            so.FindProperty("skillID").intValue = id;
            so.FindProperty("skillName").stringValue = name;
            so.FindProperty("cooldown").floatValue = cd;
            so.FindProperty("tag").enumValueIndex = (int)tag;

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(skill);
            importedSkills.Add(skill);

            string safeName = MakeSafeFileName(name);
            string expectedFileName = $"{id}_{safeName}.asset";
            string currentFileName = Path.GetFileName(AssetDatabase.GetAssetPath(skill));

            if (currentFileName != expectedFileName)
            {
                string oldPath = AssetDatabase.GetAssetPath(skill);
                string targetAssetPath = $"{folderPath}/{expectedFileName}";

                if (AssetDatabase.LoadAssetAtPath<SkillSO>(targetAssetPath) == null)
                {
                    string error = AssetDatabase.RenameAsset(oldPath, expectedFileName);
                    if (!string.IsNullOrEmpty(error))
                        Debug.LogError($"重命名失败：{error}");
                    else
                        Debug.Log($"资源名已同步：{currentFileName} → {expectedFileName}");
                }
                else
                {
                    Debug.LogWarning($"重命名跳过：目标路径已存在 {targetAssetPath}");
                }
            }

        }


        // 9. 保存与刷新
        AssetDatabase.SaveAssets();
        LuaExportService.Export(importedSkills);
        AssetDatabase.Refresh();
        Debug.Log($"导入完成：新建 {createdCount}，更新 {updatedCount}，失败 {failCount}");
        SkillEditorWindow window =EditorWindow.GetWindow<SkillEditorWindow>();
        window.LoadSkillData();
        window.Repaint();
    }

    public static string MakeSafeFileName(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name;
    }
}