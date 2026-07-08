using UnityEngine;

public enum SkillTag
{
    Attack,
    Buff,
    Control,
    Heal
}

[CreateAssetMenu()]
public class SkillSO : ScriptableObject
{
    public int skillID;
    public string skillName;
    public string filePath;
    public float cooldown;
    public SkillTag tag = SkillTag.Attack;  // 改为枚举，默认 Attack
    public BuffSO associatedBuff;       // 关联的 Buff
    public BuffTargetType buffTarget;   // Buff 目标
}