public interface IBuildStep
{
    string StepName { get; }
    BuildResult Execute();
}