using UnityEngine;

public class DefaultBuffExecutor : IBuffExecutor
{
    public void OnApply(Buff buff)
    {
        switch (buff.Config.type)
        {
            case BuffType.Attack:
                buff.Owner.AddAttack((int)(buff.Config.attackModifier * buff.CurrentStack));
                break;
            case BuffType.Poison:
            case BuffType.Heal:
            case BuffType.Shield:
                // 初始应用时无额外效果，由 OnTick 或 OnRemove 处理
                break;
        }
    }

    public void OnTick(Buff buff)
    {
        Log.Buff($"{buff.Config.buffName} Tick: 造成 {buff.Config.tickDamage * buff.CurrentStack} 点伤害");
        switch (buff.Config.type)
        {
            case BuffType.Poison:
                buff.Owner.TakeDamage((int)(buff.Config.tickDamage * buff.CurrentStack));
                break;
            case BuffType.Heal:
                buff.Owner.TakeDamage((int)(-buff.Config.tickDamage * buff.CurrentStack));
                break;
        }
    }

    public void OnStack(Buff buff)
    {
        switch (buff.Config.type)
        {
            case BuffType.Attack:
                // 先移除旧层数效果，再应用新层数效果
                buff.Owner.AddAttack((int)(-buff.Config.attackModifier * (buff.CurrentStack - 1)));
                buff.Owner.AddAttack((int)(buff.Config.attackModifier * buff.CurrentStack));
                break;
        }
    }

    public void OnRemove(Buff buff)
    {
        Log.Buff($"{buff.Config.buffName} 被移除");
        switch (buff.Config.type)
        {
            case BuffType.Attack:
                buff.Owner.AddAttack((int)(-buff.Config.attackModifier * buff.CurrentStack));
                break;
        }
    }
}