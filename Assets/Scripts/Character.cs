using System.Collections.Generic;
using UnityEngine;
using XLua;
using static UnityEngine.GraphicsBuffer;

[LuaCallCSharp]
public class Character : MonoBehaviour
{
    public FinalState health;
    public FinalState attack;
    [SerializeField] private List<SkillSO> skillConfigs;
    public string characterName;
    private int characterId;

    void Start()
    {
        characterId = GetInstanceID();

        // 向 SkillManager 注册自己的技能
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.RegisterCharacter(characterId, skillConfigs);
        }
    }

    void Awake()
    {
        // 确保在 Start 之前完成初始化
        if (health == null) health = new FinalState(100);
        if (attack == null) attack = new FinalState(10);
    }

    void Update()
    {
        // 技能释放：J键
        if (Input.GetKeyDown(KeyCode.J) && skillConfigs.Count > 0)
        {
            SkillManager.Instance.RequestCast(characterId, skillConfigs[0].skillID, this, this);
        }
       
    }

    // 属性修改方法（保持原有逻辑）
    public void AddAttack(int val) => attack.AddBonus(val);
    public void TakeDamage(int damage) => health.AddBase(-damage);

    // 给 Lua 提供简单接口，避免 Lua 直接访问 FinalState 字段
    public int GetAttackValue() => (int)attack.Value;
    public int GetHealthValue() => (int)health.Value;

    private void OnDestroy()
    {
        if (SkillManager.Instance != null)
            SkillManager.Instance.UnregisterCharacter(characterId);
    }
}