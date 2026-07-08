using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager Instance { get; private set; }
    private readonly IBuffExecutor executor = new DefaultBuffExecutor();

    void Awake() => Instance = this;

    void Update()
    {

        foreach (var runtime in SkillManager.Instance.GetAllRuntimes())
        {
            var buffs = runtime.Buffs;
            for (int i = buffs.Count - 1; i >= 0; i--)
            {
                Buff buff = buffs[i];
                int ticks = buff.Update(Time.deltaTime);

                for (int t = 0; t < ticks; t++)
                    executor.OnTick(buff);

                if (buff.IsExpired)
                {
                    executor.OnRemove(buff);
                    runtime.RemoveBuff(buff);
                }
            }
        }
    }
    public void AddBuff(Character target, BuffSO config, Character source)
    {
        var runtime = SkillManager.Instance.GetRuntime(target.GetInstanceID());
        if (runtime == null) return;

        var existing = runtime.FindBuff(config.buffID);
        if (existing != null)
        {
            existing.AddStack();
            executor.OnStack(existing);
            Log.Buff($"[BuffManager] Buff 叠加: {config.buffName}，层数 {existing.CurrentStack}");
            return;
        }

        Buff newBuff = new Buff(config, source, target);
        runtime.AddBuff(newBuff);
        executor.OnApply(newBuff);
        Log.Buff($"[BuffManager] Buff 添加: {config.buffName}，持续 {config.duration} 秒");
    }
}