using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BuffSO : ScriptableObject
{
    public string buff_name;
    public float max_duration;
    public string file_path;
    public int maxLevel;
    public float tickInterval;
}

