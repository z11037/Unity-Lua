using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class Character : MonoBehaviour
{
    public FinalState health;
    public FinalState attack;
    [SerializeField] private List<Skill> skills;
    public string charactr_name;
    public List<Buff> buffs;
    public List<DeBuff> debuffs;

    private void Start()
    {
        skills = new List<Skill>();
        buffs = new List<Buff>();

        health = new FinalState(100);
        attack = new FinalState(10);

        health.OnValueChanged += Health_OnValueChanged;
        attack.OnValueChanged += Attack_OnValueChanged;

        skills.Add(new Skill(SkillDictionary.Instance.GetSkillSO("attack")));

    }

    private void Attack_OnValueChanged(object sender, EventArgs e)
    {
        Log.State($"Attack:{attack.Value}");
    }

    private void Health_OnValueChanged(object sender, EventArgs e)
    {
        Log.State($"Health:{health.Value}");
    }

    private void Update()
    {
        UseSkill();
        ReloadLuaScript();
        CheckState();
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddBuff("poison");
        }
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            if (buffs[i].ReduceTime(Time.deltaTime, this))
            {
                buffs[i].Remove(this);
                buffs.RemoveAt(i);
            }
        }
    }

    private void CheckState()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Log.State(
                $"Health: {health.Value} | Attack: {attack.Value}"
            );
        }
    }

    private void ReloadLuaScript()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            foreach (Skill skill in skills)
            {
                LuaManager.Instance.Reload(skill.GetSkillSO().filePath);
            }

            Log.Skill("Lua Scripts Reloaded");
        }
    }

    private void UseSkill()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (skills[0].ReduceCooldown(Time.deltaTime))
            {
                skills[0].Execute(this, this);
            }
            else
            {
                Log.Skill($"{skills[0].GetSkillSO().skillName}冷却中");
            }
        }
        
    }
    public void AddAttack(int val)
    {
        attack.AddBonus(val);
    }
    public void AddBuff(string buffName)
    {
        Buff buff = buffs.Find(b => b.GetBuffSO().buff_name == buffName);
        if (buff!=null)
        {
            buff.AddLevel(1, this);
        }
        else
        {
            buff = new(BuffDictionary.Instance.GetBuffSO(buffName));
            buffs.Add(buff);
            buff.Apply(this);
        }
    }
    public void TakeDamage(int damage)
    {
        health.AddBase(-damage);
    }
}
