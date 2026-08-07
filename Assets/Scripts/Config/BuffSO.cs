using UnityEngine;

public enum BuffType
{
    Poison,   // 中毒：Tick扣血
    Heal,     // 持续回血
    Attack,   // 攻击力修正
    Shield    // 护盾（预留）
}

[CreateAssetMenu(fileName = "BuffSO", menuName = "BuffSO")]
public class BuffSO : ScriptableObject
{
    public int buffID;
    public string buffName;
    public BuffType type;
    public float duration;
    public float tickInterval;
    public int maxStack = 1;
    public float effectValue;
    private void OnValidate()
    { 

        if (tickInterval < 0f)
        {
            tickInterval = 0f;
        }

        if (maxStack < 1)
        {
            maxStack = 1;
        }
    }
}