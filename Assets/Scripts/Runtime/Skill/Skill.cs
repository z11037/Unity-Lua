using System;
using UnityEngine;
using XLua;

[CSharpCallLua]
public delegate void SkillExecute(Character attacker, Character target);

public class Skill
{
    private readonly SkillExecute skillExecute;
    public SkillSO Config { get; }

    public Skill(SkillSO config)
    {
        Config = config;
        skillExecute = LuaManager.Instance.GetSkillExecute(config.filePath);
    }

    public void Execute(Character attacker, Character target)
    {
        skillExecute?.Invoke(attacker, target);
    }
}