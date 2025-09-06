using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName = "Scriptable Objects/Stats")]
public class Stats : ScriptableObject
{
    [SerializeField] private List<Stat> stats;
    private Dictionary<StatType, float> statsDict = new Dictionary<StatType, float>();
    private bool initialised = false;

    private void Initialise() { 
        foreach (var stat in stats)
        {
            statsDict[stat.type] = stat.value;
        }
    }   

    public float GetStat(StatType statType)
    {
        if (!initialised || statsDict.Count == 0)
        {
            Initialise();
            initialised = true;
        }
        if (statsDict.ContainsKey(statType)) 
            return statsDict[statType];
        Debug.Log($"Stat of {statType} not found");
        return 0;
    }

    public List<Stat> GetAllStats()
    {
        return stats;
    }
}
