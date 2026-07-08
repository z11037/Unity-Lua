public class BuildResult
{
    public bool Success { get; }
    public int SuccessCount { get; }
    public int FailCount { get; }
    public string Message { get; }

    private BuildResult(bool success, int successCount, int failCount, string message)
    {
        Success = success;
        SuccessCount = successCount;
        FailCount = failCount;
        Message = message;
    }

    public static BuildResult Ok(int successCount = 0, string message = "")
    {
        return new BuildResult(true, successCount, 0, message);
    }

    public static BuildResult Fail(string message, int successCount = 0, int failCount = 0)
    {
        return new BuildResult(false, successCount, failCount, message);
    }
}