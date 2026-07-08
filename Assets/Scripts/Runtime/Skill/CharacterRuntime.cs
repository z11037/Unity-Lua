 using System.Collections.Generic;
using UnityEngine;

public class CharacterRuntime
{
    // 技能模块
    private readonly Dictionary<int, Skill> skills = new();
    private readonly Dictionary<int, float> cooldowns = new();
    // Buff 模块
    private readonly List<Buff> buffs = new();
    public IReadOnlyList<Buff> Buffs => buffs;


    // --- 技能相关 ---
    public void RegisterSkill(SkillSO config) => skills[config.skillID] = new Skill(config);
    public bool TryGetSkill(int id, out Skill skill) => skills.TryGetValue(id, out skill);

    public bool IsOnCooldown(int id) =>
        cooldowns.TryGetValue(id, out float endTime) && Time.time < endTime;
    public float GetRemainingCooldown(int id) =>
        cooldowns.TryGetValue(id, out float t) ? Mathf.Max(0, t - Time.time) : 0f;
    public void StartCooldown(int id, float duration) =>
        cooldowns[id] = Time.time + duration;
    // --- Buff 相关 ---
    public void AddBuff(Buff buff) => buffs.Add(buff);
    public void RemoveBuff(Buff buff) => buffs.Remove(buff);
    public Buff FindBuff(int buffID) => buffs.Find(b => b.Config.buffID == buffID);
    public bool HasBuff(int buffID) => buffs.Exists(b => b.Config.buffID == buffID);

}
