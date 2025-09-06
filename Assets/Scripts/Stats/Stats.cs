using System.Collections.Generic;
using System;
using UnityEngine;

public class Stats : MonoBehaviour
{
    private Dictionary<StatType, float> statsDict = new Dictionary<StatType, float>();
    internal Action<StatType, float> OnStatUpdated;

    public void Initialise(StatsSO statsSO)
    {
        foreach (var stat in statsSO.stats)
        {
            statsDict[stat.type] = stat.value;
        }
    }

    public float GetStat(StatType statType)
    {
        if (statsDict.ContainsKey(statType))
            return statsDict[statType];

        Debug.Log($"Stat of {statType} not found");
        return 0;
    }

    public Dictionary<StatType, float> GetAllStats()
    {
        return statsDict;
    }
}
