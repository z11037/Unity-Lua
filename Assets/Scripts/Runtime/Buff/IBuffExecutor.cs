public interface IBuffExecutor
{
    void OnApply(Buff buff);
    void OnTick(Buff buff);
    void OnStack(Buff buff);
    void OnRemove(Buff buff);
}