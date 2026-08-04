using System.Collections.Generic;
using UnityEngine;

public sealed class SkillManager : MonoBehaviour
{
    public static SkillManager Instance
    {
        get;
        private set;
    }

    private readonly Dictionary<int, CharacterRuntime> runtimes =new Dictionary<int, CharacterRuntime>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Log.Skill("[Warning] 检测到重复的 SkillManager，销毁当前对象");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Log.Skill("SkillManager 初始化完成");
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        foreach (CharacterRuntime runtime in runtimes.Values)
        {
            runtime.Tick(deltaTime);
        }
    }

    public bool RegisterCharacter( int characterId,List<SkillSO> configs)
    {
        if (runtimes.ContainsKey(characterId))
        {
            Log.Skill( $"[Warning] 角色 {characterId} 已注册，将替换旧运行时");
            UnregisterCharacter(characterId);
        }

        CharacterRuntime runtime =new CharacterRuntime(characterId);

        if (configs != null)
        {
            foreach (SkillSO config in configs)
            {
                if (config != null)
                {
                    runtime.RegisterSkill(config);
                }
                    
            }
        }

        runtimes.Add(characterId, runtime);

        Log.Skill( $"角色 {characterId} 注册完成，技能数量：{configs?.Count ?? 0}");

        return true;
    }

    public bool UnregisterCharacter(int characterId)
    {
        if (!runtimes.TryGetValue( characterId,out CharacterRuntime runtime))
        {
            Log.Skill($"[Warning] 无法注销角色 {characterId}：运行时不存在");

            return false;
        }

        runtime.Dispose();
        runtimes.Remove(characterId);

        Log.Skill( $"角色 {characterId} 已从 SkillManager 注销");

        return true;
    }

    public bool RequestCast(int characterId, int skillId,Character caster, Character target)
    {
        if (!runtimes.TryGetValue( characterId, out CharacterRuntime runtime))
        {
            Log.Skill( $"角色 {characterId} 请求释放技能 {skillId} 失败：运行时不存在");

            return false;
        }

        Log.Skill($"角色 {characterId} 请求释放技能 {skillId}");

        bool success = runtime.TryCast(skillId, caster, target);

        if (!success)
        {
            Log.Skill($"角色 {characterId} 释放技能 {skillId} 失败");
        }

        return success;
    }

    public bool TryGetRuntime( int characterId, out CharacterRuntime runtime)
    {
        return runtimes.TryGetValue( characterId, out runtime);
    }

    public CharacterRuntime GetRuntime(int characterId)
    {
        runtimes.TryGetValue( characterId, out CharacterRuntime runtime);
        return runtime;
    }

    public IEnumerable<CharacterRuntime> GetAllRuntimes()
    {
        return runtimes.Values;
    }
    private void OnDestroy()
    {
        foreach (CharacterRuntime runtime in runtimes.Values)
        {
            runtime.Dispose();
        }

        runtimes.Clear();

        if (Instance == this)
            Instance = null;

        Log.Skill("SkillManager 已销毁");
    }
}