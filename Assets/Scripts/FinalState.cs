using System;

public class FinalState
{
    public float baseValue;
    public float bonus;

    public float Value => baseValue + bonus;

    public event EventHandler OnValueChanged;

    public FinalState(float baseValue)
    {
        this.baseValue = baseValue;
    }

    public void AddBase(float val)
    {
        baseValue += val;
        OnValueChanged?.Invoke(this, EventArgs.Empty);
    }
    public void AddBonus(float val)
    {
        bonus += val;
        OnValueChanged?.Invoke(this, EventArgs.Empty);
    }
}