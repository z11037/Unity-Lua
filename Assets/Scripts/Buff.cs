using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[CSharpCallLua]
public delegate void BuffApply(Character character, int level);
[CSharpCallLua]
public delegate void BuffRemove(Character character, int level);
[CSharpCallLua]
public delegate void BuffTick(Character character, int level);
public class Buff
{
    private int current_level;
    private float current_time;
    private float tickTimer;
    private LuaTable table;
    private BuffSO buffSO;
    private BuffApply apply;
    private BuffRemove remove;
    private BuffTick tick;
    public Buff(BuffSO buffSO)
    {
        current_level = 0;
        table = LuaManager.Instance.DoString($"return require('{buffSO.file_path}')")[0] as LuaTable;
        this.buffSO = buffSO;
        current_time=buffSO.max_duration;
        tickTimer = buffSO.tickInterval;
        apply = table.Get<BuffApply>("Apply");
        remove = table.Get<BuffRemove>("Remove");
        tick = table.Get<BuffTick>("Tick");
    }

    public bool ReduceTime(float time, Character character)
    {
        current_time -= time;
        Tick(time,character);
        if (current_time <= 0)
        {
            return true;
        }

        return false;
    }

    public void Tick(float time,Character character)
    {
        if (buffSO.tickInterval <= 0) return;
        tickTimer -= time;
        if (tickTimer <= 0)
        {
            tickTimer = buffSO.tickInterval;
            tick?.Invoke(character, current_level);
        }
    }
    public void Apply(Character character)
    {
        current_time = buffSO.max_duration;
        current_level++;
        apply?.Invoke(character,1);
    }

    public void Remove(Character character)
    {
         remove?.Invoke(character,current_level);
    }

    public BuffSO GetBuffSO()
    {
        return buffSO;
    }

    public void AddLevel(int count, Character character)
    {
        int addCount = Math.Min(count, buffSO.maxLevel - current_level);

        for (int i = 0; i < addCount; i++)
        {
            Apply(character);
        }
    }
}
