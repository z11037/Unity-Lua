using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LuaExportStep : IBuildStep
{
    public string StepName => "Export Lua Templates";

    public BuildResult Execute()
    {
        return LuaExportService.ExportAll();
    }
}