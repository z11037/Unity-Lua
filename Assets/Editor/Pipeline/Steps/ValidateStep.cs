using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ValidateStep : IBuildStep
{
    public string StepName => "Validate Skill Configs";

    public BuildResult Execute()
    {
        List<SkillSO> skills = SkillRepository.LoadAll();

        Debug.Log($"加载到 {skills.Count} 个技能");

        var errors = SkillValidator.Validate(skills);


        if(errors.Count > 0)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Found {errors.Count} issue(s):");

            foreach(var error in errors)
            {
                sb.AppendLine($" - {error}");
            }

            return BuildResult.Fail(
                sb.ToString(),
                skills.Count,
                errors.Count);
        }


        return BuildResult.Ok(
            skills.Count,
            $"All {skills.Count} skills passed validation");
    }
}