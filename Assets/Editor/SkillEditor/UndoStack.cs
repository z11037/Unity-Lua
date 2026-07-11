using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class UndoStack
{
    public enum UndoActionType { Create, Delete, Restore }

    public struct UndoAction
    {
        public UndoActionType type;
        public ScriptableObject skill;
        public string originalPath;   // 删除时用：原路径
        public string recyclePath;    // 删除时用：回收站路径
        public string luaOriginalPath;//lua原路径
        public string luaRecyclePath;//lua回收站路径
    }

    private const int MaxUndoSteps = 50;
    private readonly List<List<UndoAction>> history = new();

    public void Record(List<UndoAction> actions)
    {
        if (actions == null || actions.Count == 0)
            return;
        history.Add(new List<UndoAction>(actions));
        while (history.Count > MaxUndoSteps)
        {
            history.RemoveAt(0);
            Debug.LogWarning($"撤销历史已达到上限 {MaxUndoSteps} 步，最早的操作记录已被移除");
        }
    }

    // 撤销最近一次操作，返回被撤销的操作列表
    public List<UndoAction> Undo()
    {
        if (history.Count == 0) return null;
        var last = history[history.Count - 1];
        history.RemoveAt(history.Count - 1);
        return last;
    }

    // 清空历史
    public void Clear()
    {
        history.Clear();
    }

    public bool HasUndo()
    {
        return history.Count > 0;
    }
    public void PerformUndo()
    {
        // 从撤销栈中弹出最近一次操作
        var lastActions = Undo();
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
                    {

                        // 恢复SkillSO
                        string error = AssetDatabase.MoveAsset(
                            action.recyclePath,
                            action.originalPath);

                        if (!string.IsNullOrEmpty(error))
                            Debug.LogError(error);


                        // 恢复Lua
                        if (!string.IsNullOrEmpty(action.luaRecyclePath))
                        {
                            error = AssetDatabase.MoveAsset(
                                action.luaRecyclePath,
                                action.luaOriginalPath);

                            if (!string.IsNullOrEmpty(error))
                                Debug.LogError(error);
                        }

                        break;
                    }

                case UndoStack.UndoActionType.Restore:
                    {
                        // 删除SkillSO
                        string error = AssetDatabase.MoveAsset(
                            action.originalPath,
                            action.recyclePath);

                        if (!string.IsNullOrEmpty(error))
                            Debug.LogError(error);


                        // 删除Lua
                        if (!string.IsNullOrEmpty(action.luaRecyclePath))
                        {
                            error = AssetDatabase.MoveAsset(
                                action.luaOriginalPath,
                                action.luaRecyclePath);

                            if (!string.IsNullOrEmpty(error))
                                Debug.LogError(error);
                        }
                        break;
                    }
            }
        }

        
    }

}
