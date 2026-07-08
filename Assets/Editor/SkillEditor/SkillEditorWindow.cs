using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEngine;
using UndoAction = UndoStack.UndoAction;
using UndoActionType = UndoStack.UndoActionType;

public class SkillEditorWindow : EditorWindow
{
    private HashSet<SkillSO> selectedSkills = new HashSet<SkillSO>();
    private Dictionary<SkillSO, bool> foldouts = new Dictionary<SkillSO, bool>();
    private List<SkillSO> skills = new();
    private string newSkillName = "";
    private string searchFilter = "";
    private bool sortByName = true; // true=按名字排序，false=按ID排序
    private double lastValidationTime = 0;
    private const double validationInterval = 0.5; // 每0.5秒最多校验一次
    private SkillTag tagFilter = (SkillTag)(-1); // -1 = All
    private RecycleBinArea recycleBinArea;
    // 记录每次删除操作涉及的文件路径映射：原路径 → 回收站路径
    private string recycleBinPath = SkillPathConfig.RecycleBin;
    private string lastValidationLog = "";

    private readonly UndoStack unifiedUndoStack = new();

    [MenuItem("Tools/技能编辑器")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<SkillEditorWindow>("技能编辑器");

    }

    private void OnEnable()
    {
        LoadSkillData();
        recycleBinArea = new RecycleBinArea(recycleBinPath, this);
    }

    private void OnGUI()
    {
        DrawToolbar();                    // 刷新、删除选中
        DrawCreateSkillPanel();           // 新增技能输入框 + 添加按钮
        DrawValidationAndExportButtons(); // 校验、保存、导出Lua
        DrawSkillList();                  // 技能折叠列表
        DrawSelectedInfo();               // 当前选中显示
        recycleBinArea.Draw();
        // OnGUI 末尾，自动校验
        if (EditorApplication.timeSinceStartup - lastValidationTime > validationInterval)
        {
            string currentLog = "";
            var errors = SkillValidator.Validate(skills);

            if (errors.Count == 0)
            {
                currentLog = "✅ 所有配置校验通过。";
            }
            else
            {
                // 将所有错误信息拼接成一个字符串，用作对比
                currentLog = string.Join("\n", errors);
            }

            // 结果不变时不重复输出
            if (currentLog != lastValidationLog)
            {
                lastValidationLog = currentLog;
                if (errors.Count == 0)
                    Debug.Log(currentLog);
                else
                    foreach (string err in errors)
                        Debug.LogWarning(err);
            }

            lastValidationTime = EditorApplication.timeSinceStartup;
        }
        HandleUndoShortcut();
    }

    private void HandleUndoShortcut()
    {
        Event e = Event.current;

        if (e.type == EventType.KeyDown &&
            e.control &&
            e.keyCode == KeyCode.Z)
        {
            PerformUndo();
            e.Use();
        }
    }

    private void DrawToolbar()
    {
        if (GUILayout.Button("刷新列表"))
        {
            LoadSkillData();
            selectedSkills.Clear();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("删除选中"))
        {
            DeleteSelectedSkills();
        }
        EditorGUILayout.Space();

        if (GUILayout.Button("清空回收站"))
        {
            ClearRecycleBin();
        }
    }
    private void ClearRecycleBin()
    {
        if (System.IO.Directory.Exists(SkillPathConfig.RecycleBin))
        {
            // 确认对话框，防止误触
            if (EditorUtility.DisplayDialog(
                "清空回收站",
                "确定要永久删除回收站中的所有技能吗？此操作不可撤销。",
                "确定",
                "取消"))
            {
                // 删除回收站文件夹
                System.IO.Directory.Delete(recycleBinPath, true);
                // 删除对应的 .meta 文件
                string metaPath = recycleBinPath + ".meta";
                if (System.IO.File.Exists(metaPath))
                    System.IO.File.Delete(metaPath);

                AssetDatabase.Refresh();
                Debug.Log("回收站已清空。");
            }
        }
        else
        {
            Debug.Log("回收站已空。");
        }
    }
    private void DeleteSelectedSkills()
    {
        if (selectedSkills.Count == 0) return;

        var skillsToDelete = new List<SkillSO>(selectedSkills);

        // 确保回收站文件夹存在
        if (!AssetDatabase.IsValidFolder(recycleBinPath))
        {
            string parent = System.IO.Path.GetDirectoryName(recycleBinPath);
            string folder = System.IO.Path.GetFileName(recycleBinPath);
            AssetDatabase.CreateFolder(parent, folder);
        }

        // 记录本次操作的所有撤销信息
        var actions = new List<UndoStack.UndoAction>();

        foreach (SkillSO skillToDelete in skillsToDelete)
        {
            // 移动 .asset 文件到回收站
            string currentPath = AssetDatabase.GetAssetPath(skillToDelete);
            string fileName = System.IO.Path.GetFileName(currentPath);
            string recyclePath = recycleBinPath + "/" + fileName;

            // 处理重名
            int counter = 1;
            while (System.IO.File.Exists(recyclePath))
            {
                string nameNoExt = System.IO.Path.GetFileNameWithoutExtension(fileName);
                recyclePath = recycleBinPath + "/" + nameNoExt + " " + counter + ".asset";
                counter++;
            }

            AssetDatabase.MoveAsset(currentPath, recyclePath);

            var undoAction = new UndoAction
            {
                type = UndoActionType.Delete,
                skill = skillToDelete,
                originalPath = currentPath,
                recyclePath = recyclePath,  // recyclePath 是上面计算好的实际路径
            };

            // 移动关联的 Lua 文件及其 .meta 文件
            if (!string.IsNullOrEmpty(skillToDelete.filePath))
            {
                string luaCurrentPath =Path.Combine(Application.dataPath,skillToDelete.filePath.Substring("Assets/".Length));
                if (System.IO.File.Exists(luaCurrentPath))
                {
                    string luaFileName = System.IO.Path.GetFileName(luaCurrentPath);
                    string luaRecyclePath = Path.Combine(Application.dataPath,recycleBinPath.Substring("Assets/".Length),luaFileName);
                    System.IO.File.Move(luaCurrentPath, luaRecyclePath);

                    // 同步移动 .meta 文件
                    string luaMetaCurrentPath = luaCurrentPath + ".meta";
                    if (System.IO.File.Exists(luaMetaCurrentPath))
                    {
                        string luaMetaRecyclePath = luaRecyclePath + ".meta";
                        System.IO.File.Move(luaMetaCurrentPath, luaMetaRecyclePath);
                    }
                    undoAction.luaOriginalPath = luaCurrentPath;
                    undoAction.luaRecyclePath = luaRecyclePath;
                }
            }
            // 记录本次操作，用于撤销
            actions.Add(undoAction);
            // 从内存列表移除
            selectedSkills.Remove(skillToDelete);
            skills.Remove(skillToDelete);
            foldouts.Remove(skillToDelete);
        }

        // 把本次所有删除操作压入统一撤销栈
        unifiedUndoStack.Record(actions);

        AssetDatabase.Refresh();
        LoadSkillData();
        Repaint();
    }
    private void DrawCreateSkillPanel()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("新增技能", EditorStyles.boldLabel);
        newSkillName = EditorGUILayout.TextField("技能名称", newSkillName);
        if (GUILayout.Button("添加技能"))
        {
            CreateNewSkill();
        }
    }

    private void CreateNewSkill()
    {
        if (!string.IsNullOrWhiteSpace(newSkillName))
        {
            string folderPath = SkillPathConfig.SkillFolder;
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }

            SkillSO newSkill = CreateInstance<SkillSO>();
            int maxID = skills.Count > 0 ? skills.Max(s => s.skillID) : 1000;
            newSkill.skillID = maxID + 1;
            newSkill.skillName = newSkillName.Trim();

            string assetName = newSkillName.Trim();
            string targetPath = $"{folderPath}/{newSkill.skillID}_{assetName}.asset";
            int counter = 1;
            while (System.IO.File.Exists(targetPath))
            {
                targetPath = $"{folderPath}/{assetName} {counter}.asset";
                counter++;
            }

            
            skills.Add(newSkill);
            AssetDatabase.CreateAsset(newSkill, targetPath);
            AssetDatabase.SaveAssets();
            LuaExportService.Export(new List<SkillSO>() { newSkill });
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(newSkill);
            newSkillName = "";
            GUI.FocusControl(null);
            var action = new UndoAction
            {
                type = UndoActionType.Create,
                skill = newSkill
            };
            unifiedUndoStack.Record(new List<UndoAction> { action });
        }

    }
    private void DrawValidationAndExportButtons()
    {
        if (GUILayout.Button("保存修改"))
        {
            AssetDatabase.SaveAssets();
            Debug.Log("技能数据已保存。");
        }

        if (GUILayout.Button("校验配置"))
        {
            ValidateSkills();
            
        }

        if (GUILayout.Button("生成SkillID枚举"))
        {
            GenerateSkillIdEnum();
        }

        if (GUILayout.Button("Lua脚本导出"))
        {
            ExportLuaTemplates();
        }
    }

    private void GenerateSkillIdEnum()
{
    string enumPath = Application.dataPath + "/SkillID.cs";
    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(enumPath))
    {
        sw.WriteLine("public enum SkillID");
        sw.WriteLine("{");
        for (int i = 0; i < skills.Count; i++)
        {
            string comma = (i == skills.Count - 1) ? "" : ",";
            // 去掉名字中的空格，防止非法标识符
            string enumName = skills[i].skillName.Replace(" ", "").Replace("-", "_");
            sw.WriteLine($"    {enumName} = {skills[i].skillID}{comma}");
        }
        sw.WriteLine("}");
    }
    AssetDatabase.Refresh();
    Debug.Log($"SkillID 枚举已生成到 Assets/SkillID.cs");
}
    private void ExportLuaTemplates()
    {
        LuaExportService.Export(skills);
    }
    private void ValidateSkills()
    {
        var errors = SkillValidator.Validate(skills);
        if (errors.Count == 0)
            Debug.Log("所有配置校验通过。");
        else
            foreach (string err in errors)
                Debug.LogWarning(err);
    }

    private void DrawSkillList()
    {
        string[] options = System.Enum.GetNames(typeof(SkillTag));
        int index = (int)tagFilter + 1; // +1 因为有“全部”

        string[] display = new string[options.Length + 1];
        display[0] = "All";
        System.Array.Copy(options, 0, display, 1, options.Length);

        index = EditorGUILayout.Popup("Tag", index, display);

        tagFilter = (SkillTag)(index - 1);

        EditorGUILayout.BeginHorizontal();
        searchFilter = EditorGUILayout.TextField("搜索技能", searchFilter);
        if (GUILayout.Button(sortByName ? "当前：名字排序" : "当前：ID排序", GUILayout.Width(120)))
        {
            sortByName = !sortByName;
            skills.Sort(sortByName ?
                (a, b) => a.skillName.CompareTo(b.skillName) :
                (a, b) => a.skillID.CompareTo(b.skillID));


        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        if (skills.Count == 0)
        {
            EditorGUILayout.HelpBox("尚未创建任何技能数据资产。", MessageType.Warning);
            return;
        }
        for (int i = 0; i < skills.Count; i++)
        {
            if (!string.IsNullOrEmpty(searchFilter) && !skills[i].skillName.ToLower().Contains(searchFilter.ToLower()))
            {
                continue;
            }
            if ((int)tagFilter != -1 && skills[i].tag != tagFilter)
            {
                continue;
            }

            DrawSkillItem(skills[i]);
        }
        
    }

    public void RestoreFromRecycleBin(SkillSO recycleSkill)
    {
        string currentPath = AssetDatabase.GetAssetPath(recycleSkill);

        string fileName =System.IO.Path.GetFileName(currentPath);

        string targetPath = SkillPathConfig.SkillFolder + "/" + fileName;

        int counter = 1;

        while (System.IO.File.Exists(targetPath))
        {
            string name =
                System.IO.Path.GetFileNameWithoutExtension(fileName);

            targetPath = SkillPathConfig.SkillFolder +"/" +name + " " + counter +".asset";

            counter++;
        }

        AssetDatabase.MoveAsset(currentPath,targetPath);

        // 恢复Lua文件
        RestoreLuaFile(recycleSkill);

        AssetDatabase.Refresh();

        LoadSkillData();

        Repaint();
    }
    private void RestoreLuaFile(SkillSO skill)
    {
        if (string.IsNullOrEmpty(skill.filePath))
            return;


        string luaName =
            Path.GetFileName(skill.filePath);


        string recycleLua =
            Path.Combine(
                Application.dataPath,
                SkillPathConfig.RecycleBin.Substring("Assets/".Length),
                luaName);


        if (!File.Exists(recycleLua))
        {
            Debug.LogWarning(
                "找不到回收站Lua:" + recycleLua);
            return;
        }


        string targetLua =
            Path.Combine(
                Application.dataPath,
                skill.filePath.Substring("Assets/".Length));


        File.Move(
            recycleLua,
            targetLua);


        string recycleMeta =
            recycleLua + ".meta";


        if (File.Exists(recycleMeta))
        {
            File.Move(
                recycleMeta,
                targetLua + ".meta");
        }
    }

    private void DrawSkillItem(SkillSO skill)
    {
        if (skill == null) return;
        if (!foldouts.ContainsKey(skill)) foldouts[skill] = false;

        SerializedObject so = new SerializedObject(skill);
        so.Update();

        SerializedProperty nameProp = so.FindProperty("skillName");
        SerializedProperty cooldownProp = so.FindProperty("cooldown");
        SerializedProperty tagProp = so.FindProperty("tag");
        SerializedProperty buffProp = so.FindProperty("associatedBuff");
        SerializedProperty targetProp = so.FindProperty("buffTarget");

        EditorGUILayout.BeginHorizontal();

        // 多选复选框
        bool wasSelected = selectedSkills.Contains(skill);
        bool isSelected = EditorGUILayout.Toggle(wasSelected, GUILayout.Width(20));
        if (isSelected && !wasSelected) selectedSkills.Add(skill);
        else if (!isSelected && wasSelected) selectedSkills.Remove(skill);

        EditorGUILayout.BeginVertical();

        // 折叠面板 + 一键定位
        EditorGUILayout.BeginHorizontal();
        foldouts[skill] = EditorGUILayout.Foldout(foldouts[skill], $"[{skill.skillID}] {skill.skillName}");
        if (GUILayout.Button("", GUILayout.Width(25)))
        {
            EditorGUIUtility.PingObject(skill);
        }
        
        EditorGUILayout.EndHorizontal();

        // 展开后的编辑区域
        if (foldouts[skill])
        {
            EditorGUI.BeginChangeCheck();

            // ID（只读）
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.IntField("ID", skill.skillID);
            EditorGUI.EndDisabledGroup();

            // 可编辑字段
            if (tagProp != null) EditorGUILayout.PropertyField(tagProp);
            EditorGUILayout.PropertyField(nameProp);
            EditorGUILayout.PropertyField(cooldownProp);

            //lua脚本路径
            EditorGUILayout.LabelField(
                "Lua脚本",
                string.IsNullOrEmpty(skill.filePath)
                    ? "未绑定"
                    : skill.filePath);
            if (GUILayout.Button("绑定Lua脚本"))
            {
                BindLuaScript(skill);
            }

            if (buffProp != null)
            {
                EditorGUILayout.PropertyField(buffProp);

                // 只有选了 Buff 才显示目标选择
                if (buffProp.objectReferenceValue != null && targetProp != null)
                    EditorGUILayout.PropertyField(targetProp);
            }

            if (EditorGUI.EndChangeCheck())
            {
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(skill);
            }

          
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    private void BindLuaScript(SkillSO skill)
    {
        string absolutePath =
            EditorUtility.OpenFilePanel(
                "选择Lua脚本",
                Application.dataPath,
                "lua");


        if (string.IsNullOrEmpty(absolutePath))
            return;


        string assetPath =
            "Assets" +
            absolutePath
            .Replace(Application.dataPath, "")
            .Replace("\\", "/");

        skill.filePath = assetPath;

        EditorUtility.SetDirty(skill);

        AssetDatabase.SaveAssets();
    }
    private void DrawSelectedInfo()
    {
        if (selectedSkills.Count > 0)
        {
            EditorGUILayout.Space();
            var selectedNames = new List<string>();
            foreach (SkillSO skill in selectedSkills)
            {
                if (skill != null)
                    selectedNames.Add(skill.skillName);
            }
            EditorGUILayout.LabelField("当前选中", string.Join(", ", selectedNames));
        }
    }

    public void LoadSkillData()
    {
        skills = SkillRepository.LoadAll();
    }

    private void PerformUndo()
    {
        Debug.Log("PerformUndo");
        // 从撤销栈中弹出最近一次操作
        var lastActions = unifiedUndoStack.Undo();
        Debug.Log(lastActions == null ? "Undo=null" : $"Undo={lastActions.Count}");

        // 栈为空，无需处理
        if (lastActions == null)
            return;

        foreach (var action in lastActions)
        {
            switch (action.type)
            {
                case UndoStack.UndoActionType.Create:
                    // 撤销“创建技能”：删除对应的资产文件
                    string path = AssetDatabase.GetAssetPath(action.skill);
                    if (!string.IsNullOrEmpty(path))
                        AssetDatabase.DeleteAsset(path);
                    break;

                case UndoStack.UndoActionType.Delete:
                    // 撤销“删除技能”：将文件从回收站移回原路径
                    string error = AssetDatabase.MoveAsset(
    action.recyclePath,
    action.originalPath);

                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.LogError(error);
                    }
                    // 恢复 Lua
                    if (!string.IsNullOrEmpty(action.luaRecyclePath) &&
                        File.Exists(action.luaRecyclePath))
                    {
                        string directory =
                            Path.GetDirectoryName(action.luaOriginalPath);

                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        File.Move(
                            action.luaRecyclePath,
                            action.luaOriginalPath);
                    }


                    // 恢复 Lua meta
                    string luaMetaRecycle =
                        action.luaRecyclePath + ".meta";

                    string luaMetaOriginal =
                        action.luaOriginalPath + ".meta";


                    if (File.Exists(luaMetaRecycle))
                    {
                        File.Move(
                            luaMetaRecycle,
                            luaMetaOriginal);
                    }
                    break;

                case UndoStack.UndoActionType.Edit:
                    // 编辑操作由 SerializedObject 原生处理，这里不需要额外逻辑
                    break;
            }
        }

        // 刷新数据并重绘UI
        LoadSkillData();
        Repaint();
    }
}