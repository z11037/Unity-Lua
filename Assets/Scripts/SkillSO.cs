using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SkillSO:ScriptableObject
{
    public string skillName;
    public string filePath;
    public float cooldown;
}
