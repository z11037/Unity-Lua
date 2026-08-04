using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class CharacterRuntime : IDisposable
{
    public int CharacterId { get; private set; }

    private readonly Dictionary<int, Skill> skills = new Dictionary<int, Skill>();
    private readonly List<Buff> buffs = new List<Buff>();

    public List<Buff> Buffs
    {
        get
        {
            return buffs;
        }
    }

    public CharacterRuntime(int characterId)
    {
        CharacterId = characterId;
    }

    public bool RegisterSkill(SkillSO config)
    {
        if (config == null)
        {
            Log.Skill($"[Warning] 角色 {CharacterId} 注册技能失败：配置为空");
            return false;
        }

        int skillId = config.skillID;

        if (skills.ContainsKey(skillId))
        {
            Log.Skill($"[Warning] 角色 {CharacterId} 已经注册技能 {skillId}");
            return false;
        }

        try
        {
            Skill skill = new Skill(config);

            if (!skill.IsReady)
            {
                Log.Skill($"[Error] 角色 {CharacterId} 注册技能 {skillId} 失败：Lua 技能未正确加载");
                skill.Dispose();
                return false;
            }

            skills.Add(skillId, skill);
            Log.Skill($"角色 {CharacterId} 注册技能 {skillId} 成功");
            return true;
        }
        catch (Exception exception)
        {
            Log.Skill($"[Error] 角色 {CharacterId} 创建技能 {skillId} 失败");
            Debug.LogException(exception);
            return false;
        }
    }

    public bool UnregisterSkill(int skillId)
    {
        if (!skills.TryGetValue(skillId, out Skill skill))
        {
            return false;
        }

        skill.Dispose();
        skills.Remove(skillId);

        Log.Skill($"角色 {CharacterId} 已移除技能 {skillId}");
        return true;
    }

    public bool TryGetSkill(int skillId, out Skill skill)
    {
        return skills.TryGetValue(skillId, out skill);
    }

    public bool TryCast(int skillId, Character caster, Character target)
    {
        if (!skills.TryGetValue(skillId, out Skill skill))
        {
            Log.Skill($"角色 {CharacterId} 未注册技能 {skillId}");
            return false;
        }

        return skill.TryExecute(caster, target);
    }

    public float GetRemainingCooldown(int skillId)
    {
        if (!skills.TryGetValue(skillId, out Skill skill))
        {
            return 0f;
        }

        return skill.CurrentCooldown;
    }

    public bool IsOnCooldown(int skillId)
    {
        if (!skills.TryGetValue(skillId, out Skill skill))
        {
            return false;
        }

        return !skill.CanExecute;
    }

    public Buff FindBuff(int buffId)
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            Buff buff = buffs[i];

            if (buff == null)
            {
                continue;
            }

            if (buff.Config == null)
            {
                continue;
            }

            if (buff.Config.buffID == buffId)
            {
                return buff;
            }
        }

        return null;
    }

    public Buff FindBuff(BuffSO config)
    {
        if (config == null)
        {
            return null;
        }

        return FindBuff(config.buffID);
    }

    public void AddBuff(Buff buff)
    {
        if (buff == null)
        {
            Log.Buff("[Warning] 无法添加空 Buff");
            return;
        }

        if (buffs.Contains(buff))
        {
            Log.Buff($"[Warning] 角色 {CharacterId} 已经持有 Buff {buff.Config.buffID}");
            return;
        }

        buffs.Add(buff);
        Log.Buff($"角色 {CharacterId} 添加 Buff {buff.Config.buffID}");
    }

    public bool RemoveBuff(Buff buff)
    {
        if (buff == null)
        {
            return false;
        }

        bool removed = buffs.Remove(buff);

        if (removed)
        {
            Log.Buff($"角色 {CharacterId} 移除 Buff {buff.Config.buffID}");
        }

        return removed;
    }

    public void Tick(float deltaTime)
    {
        foreach (Skill skill in skills.Values)
        {
            skill.Tick(deltaTime);
        }
    }

    public void Dispose()
    {
        foreach (Skill skill in skills.Values)
        {
            skill.Dispose();
        }

        skills.Clear();

        for (int i = 0; i < buffs.Count; i++)
        {
            Buff buff = buffs[i];

            if (buff is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        buffs.Clear();

        Debug.Log($"角色 {CharacterId} 的 CharacterRuntime 已释放");
    }
}