using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }
    private Dictionary<int, CharacterRuntime> runtimes = new();

    void Awake() => Instance = this;

    public void RegisterCharacter(int id, List<SkillSO> configs)
    {
        var runtime = new CharacterRuntime();
        foreach (var cfg in configs) runtime.RegisterSkill(cfg);
        runtimes[id] = runtime;
    }

    public void UnregisterCharacter(int id)
    {
        runtimes.Remove(id);
    }

    public bool RequestCast(int characterId, int skillID, Character caster, Character target)
    {
        if (!runtimes.TryGetValue(characterId, out var rt))
            return false;
        if (!rt.TryGetSkill(skillID, out var skill))
            return false;
        if (rt.IsOnCooldown(skillID))
        {
            Log.Skill($"技能 {skillID} 冷却中，剩余 {rt.GetRemainingCooldown(skillID):F1} 秒");
            return false;
        }
        rt.StartCooldown(skillID, skill.Config.cooldown);
        Log.Skill($"技能 {skillID} 开始执行");
        skill.Execute(caster, target);
        Log.Skill($"技能 {skillID} 执行完成");
        if (skill.Config.associatedBuff != null)
        {
            Character receiver = skill.Config.buffTarget == BuffTargetType.Self ? caster : target;
            BuffManager.Instance.AddBuff(receiver, skill.Config.associatedBuff, caster);
        }
        return true;
    }

    // 获取所有角色运行时（供 BuffManager 遍历所有角色的 Buff）
    public IEnumerable<CharacterRuntime> GetAllRuntimes()
    {
        return runtimes.Values;
    }

    // 获取指定角色的运行时（供 BuffManager 添加 Buff 时使用）
    public CharacterRuntime GetRuntime(int characterId)
    {
        runtimes.TryGetValue(characterId, out var runtime);
        return runtime;
    }
}