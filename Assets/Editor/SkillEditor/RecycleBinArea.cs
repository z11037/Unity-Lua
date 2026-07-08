using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class RecycleBinArea
{
    private string recycleBinPath;
    private SkillEditorWindow parentWindow;

    public RecycleBinArea(string path, SkillEditorWindow parent)
    {
        recycleBinPath = path;
        parentWindow = parent;
    }

    public void Draw()
    {
        if (!System.IO.Directory.Exists(recycleBinPath))
            return;

        string[] recycleGuids = AssetDatabase.FindAssets("t:SkillSO", new[] { recycleBinPath });
        if (recycleGuids.Length == 0)
            return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("ªÿ ’’æ", EditorStyles.boldLabel);

        foreach (string guid in recycleGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            SkillSO recycleSkill = AssetDatabase.LoadAssetAtPath<SkillSO>(path);
            if (recycleSkill == null) continue;

            Color oldColor = GUI.color;
            GUI.color = Color.gray;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"[{recycleSkill.skillID}] {recycleSkill.skillName} (“—…æ≥˝)");

            if (GUILayout.Button("ª÷∏¥", GUILayout.Width(50)))
            {
                parentWindow.RestoreFromRecycleBin(recycleSkill);
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = oldColor;
        }
    }
}