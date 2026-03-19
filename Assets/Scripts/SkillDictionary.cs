using System.Collections.Generic;
using UnityEngine;

public class SkillDictionary : MonoBehaviour
{
    [SerializeField] private List<SkillSO> skillList;

    private Dictionary<string, SkillSO> dic;

    public static SkillDictionary Instance;

    private void Awake()
    {
        Instance = this;

        dic = new Dictionary<string, SkillSO>();

        foreach (var skill in skillList)
        {
            dic.Add(skill.skillName, skill);
        }
    }

    public SkillSO GetSkillSO(string name)
    {
        return dic[name];
    }
}