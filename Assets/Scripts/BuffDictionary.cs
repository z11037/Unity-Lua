using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDictionary : MonoBehaviour
{
    [SerializeField] private List<BuffSO> buffList;

    private Dictionary<string, BuffSO> dic;

    public static BuffDictionary Instance;

    private void Awake()
    {
        Instance = this;

        dic = new();

        foreach (var buff in buffList)
        {
            dic.Add(buff.buff_name, buff);
        }
    }

    public BuffSO GetBuffSO(string name)
    {
        return dic[name];
    }
}
