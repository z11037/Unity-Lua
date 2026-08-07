using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XLua;

[LuaCallCSharp]
public class Character : MonoBehaviour
{
    [FormerlySerializedAs("health")]
    [SerializeField] private FinalState initialMaxHealth;

    [FormerlySerializedAs("attack")]
    [SerializeField] private FinalState initialAttack;

    [SerializeField] private List<SkillSO> skillConfigs = new List<SkillSO>();

    public string characterName;

    private int characterId;

    private void Awake()
    {
        characterId = GetInstanceID();

        if (initialMaxHealth == null)
        {
            initialMaxHealth = new FinalState(100);
        }

        if (initialAttack == null)
        {
            initialAttack = new FinalState(10);
        }
    }

    private void Start()
    {
        if (SkillManager.Instance == null)
        {
            Log.Skill($"[Warning] ½ÇÉ« {characterId} ×¢²áÊ§°Ü£ºSkillManager ÉÐÎ´³õÊ¼»¯");
            return;
        }

        SkillManager.Instance.RegisterCharacter(characterId, skillConfigs, initialMaxHealth.Value, initialAttack.Value);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.J))
        {
            return;
        }

        if (SkillManager.Instance == null)
        {
            return;
        }

        if (skillConfigs == null || skillConfigs.Count == 0)
        {
            return;
        }

        SkillSO skillConfig = skillConfigs[0];

        if (skillConfig == null)
        {
            return;
        }

        SkillManager.Instance.RequestCast(characterId, skillConfig.skillID, this, this);
    }

    public void TakeDamage(int damage)
    {
        CharacterRuntime runtime = GetRuntime();

        if (runtime == null)
        {
            return;
        }

        runtime.TakeDamage(damage);
    }

    public void Heal(int amount)
    {
        CharacterRuntime runtime = GetRuntime();

        if (runtime == null)
        {
            return;
        }

        runtime.Heal(amount);
    }

    public void AddAttack(int value)
    {
        CharacterRuntime runtime = GetRuntime();

        if (runtime == null)
        {
            return;
        }

        runtime.AddAttack(value);
    }

    public int GetAttackValue()
    {
        CharacterRuntime runtime = GetRuntime();

        if (runtime == null)
        {
            return Mathf.FloorToInt(initialAttack.Value);
        }

        return runtime.GetAttackValue();
    }

    public int GetHealthValue()
    {
        CharacterRuntime runtime = GetRuntime();

        if (runtime == null)
        {
            return Mathf.FloorToInt(initialMaxHealth.Value);
        }

        return runtime.GetHealthValue();
    }

    public int GetMaxHealthValue()
    {
        CharacterRuntime runtime = GetRuntime();

        if (runtime == null)
        {
            return Mathf.FloorToInt(initialMaxHealth.Value);
        }

        return runtime.GetMaxHealthValue();
    }

    public bool IsDead()
    {
        CharacterRuntime runtime = GetRuntime();
        return runtime != null && runtime.IsDead;
    }

    public CharacterRuntime GetRuntime()
    {
        if (SkillManager.Instance == null)
        {
            return null;
        }

        return SkillManager.Instance.GetRuntime(characterId);
    }

    private void OnDestroy()
    {
        if (SkillManager.Instance == null)
        {
            return;
        }

        SkillManager.Instance.UnregisterCharacter(characterId);
    }
}