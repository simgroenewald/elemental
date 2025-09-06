using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "StatIcons", menuName = "Scriptable Objects/StatIcons")]
public class StatIcons : ScriptableObject
{
    [SerializeField]
    private List<StatIcon> statsIcons;
    private Dictionary<StatType, Sprite> statsIconsDict = new Dictionary<StatType, Sprite>();
    private bool initialised = false;

    private void Initialise()
    {
        foreach (var statIcon in statsIcons)
        {
            statsIconsDict[statIcon.type] = statIcon.icon;
        }
    }
    public Sprite GetIcon(StatType statType)
    {
        if (!initialised || statsIconsDict.Count == 0)
        {
            Initialise();
            initialised = true;
        }

        if (statsIconsDict.ContainsKey(statType))
            return statsIconsDict[statType];

        Debug.Log($"Stat of {statType} not found");
        return null;
    }
}

[Serializable]
public class StatIcon
{
    public StatType type;
    public Sprite icon;
}