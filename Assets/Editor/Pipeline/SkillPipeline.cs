using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SkillPipeline
{
    [MenuItem("Tools/技能/技能配置构建")]
    public static void Build()
    {
        var steps = new List<IBuildStep>
        {
            new ValidateStep(),
            new SkillIdEnumStep(),
            new LuaExportStep(),
        };

        Debug.Log("========== Pipeline Start ==========");
        Stopwatch totalWatch = Stopwatch.StartNew();

        int totalSuccess = 0;
        int totalFail = 0;
        bool allSuccess = true;

        foreach (var step in steps)
        {
            Stopwatch stepWatch = Stopwatch.StartNew();

            BuildResult result;
            try
            {
                result = step.Execute();
            }
            catch (Exception e)
            {
                result = BuildResult.Fail(e.Message);
            }

            stepWatch.Stop();

            totalSuccess += result.SuccessCount;
            totalFail += result.FailCount;

            if (result.Success)
            {
                Debug.Log($"✔ [{step.StepName}] {result.Message} ({stepWatch.ElapsedMilliseconds} ms)");
            }
            else
            {
                Debug.LogError($"✘ [{step.StepName}] {result.Message} ({stepWatch.ElapsedMilliseconds} ms)");
                allSuccess = false;
                break;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        totalWatch.Stop();
        string status = allSuccess ? "Success" : "Failed";
        Debug.Log($"========== Pipeline {status} | OK: {totalSuccess} | Fail: {totalFail} | Total: {totalWatch.ElapsedMilliseconds} ms ==========");
    }
}