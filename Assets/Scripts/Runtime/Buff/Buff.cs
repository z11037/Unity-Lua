using UnityEngine;

public class Buff
{
    public BuffSO Config { get; private set; }
    public int CurrentStack { get; private set; }
    public float RemainingTime { get; private set; }
    public float TickAccumulator { get; private set; }

    // 直接持有 Character 引用
    public Character Owner { get; private set; }
    public Character Source { get; private set; }

    // Tick 触发标记
    public bool CanConsumeTick => Config.tickInterval > 0 && TickAccumulator >= Config.tickInterval;
    public bool IsExpired => RemainingTime <= 0;

    public Buff(BuffSO config, Character source, Character owner)
    {
        Config = config;
        Source = source;
        Owner = owner;
        CurrentStack = 1;
        RemainingTime = config.duration;
        TickAccumulator = 0f;
    }

    public int Update(float deltaTime)
    {
        RemainingTime -= deltaTime;

        if (Config.tickInterval <= 0)
            return 0;

        TickAccumulator += deltaTime;

        int tickCount = 0;
        while (TickAccumulator >= Config.tickInterval)
        {
            TickAccumulator -= Config.tickInterval;
            tickCount++;
        }

        return tickCount;
    }


    public void AddStack()
    {
        if (CurrentStack < Config.maxStack)
            CurrentStack++;
        RemainingTime = Config.duration; // 刷新持续时间
    }
}