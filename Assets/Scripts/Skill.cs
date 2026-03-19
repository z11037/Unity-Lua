using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[CSharpCallLua]
public delegate void SkillExecute(Character atttacker, Character character);
public class Skill
{
    private LuaTable table;
    private float cooldown;
    private SkillSO skillSO;
    private int currentLevel;
    private SkillExecute skillExecute;
    public Skill(SkillSO skillSO)
    {
        this.skillSO=skillSO;
        table = LuaManager.Instance.DoString($"return require('{skillSO.filePath}')")[0] as LuaTable;
        skillExecute = table.Get<SkillExecute>("Execute");
    }

    public void Execute(Character atttacker,Character character)
    {
        skillExecute?.Invoke(atttacker,character);
        cooldown = skillSO.cooldown;
    }

    public bool ReduceCooldown(float time)
    {
        cooldown -= time;
        return cooldown <= 0;
    }

    public SkillSO GetSkillSO() { 
        return skillSO;
    }
}
