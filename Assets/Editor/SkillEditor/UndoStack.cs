using System.Collections.Generic;
using UnityEngine;
public class UndoStack
{
    public enum UndoActionType { Create, Delete, Edit }

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
}
