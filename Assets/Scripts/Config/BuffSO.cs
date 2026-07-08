using UnityEngine;

public enum BuffType
{
    Poison,   // 中毒：Tick扣血
    Heal,     // 持续回血
    Attack,   // 攻击力修正
    Shield    // 护盾（预留）
}

[CreateAssetMenu(fileName = "BuffSO", menuName = "技能系统/BuffSO")]
public class BuffSO : ScriptableObject
{
    public int buffID;
    public string buffName;
    public BuffType type;
    public float duration;
    public float tickInterval;
    public int maxStack = 1;
    public float tickDamage;
    public float attackModifier;
}