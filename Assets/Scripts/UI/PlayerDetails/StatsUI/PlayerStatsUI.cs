using DarkPixelRPGUI.Scripts.UI.Equipment;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private StatUI statUIPrefab;
    [SerializeField] private RectTransform playerStats;
    private Dictionary<StatType, StatUI> statUIDict = new Dictionary<StatType, StatUI>();

    public void Initialise(Dictionary<StatType, float> statDict)
    {
        foreach (var stat in statDict)
        {
            StatUI statUI = Instantiate(statUIPrefab, playerStats);
            statUIDict[stat.Key] = statUI;
        }
    }

    public void SetStatUI(StatType statType, Sprite image, float value)
    {
        if (statUIDict.ContainsKey(statType))
        {
            string statname = HelperUtilities.GetDescription(statType);
            statUIDict[statType].SetStat(image, statname, value);
        }
    }

    public void UpdateStatUI(StatType statType, float value)
    {
        if (statUIDict.ContainsKey(statType))
        {
            statUIDict[statType].UpdateStat(value);
        }
    }

    internal void ResetAllStats()
    {
        foreach (var stat in statUIDict)
        {
            //statUIDict[stat.Key].ResetSlot();
        }
    }
}
