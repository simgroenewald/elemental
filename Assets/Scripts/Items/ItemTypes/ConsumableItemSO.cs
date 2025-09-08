using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "ConsumableItemSO", menuName = "Item Types/ConsumableItemSO")]
public class ConsumableItemSO : ItemSO, IDestroyableItem, IItemAction
{
    [SerializeField]
    private List<ModifierData> modifiersData = new List<ModifierData>();
    public string ActionName => "Use";

    public AudioClip actionSFX {get; private set;}

    public bool PerformAction(GameObject player, List<ItemParameter> itemParameters = null)
    {
        foreach (ModifierData data in modifiersData)
        {
            data.statModifier.AffectPlayer(player, data.statType, data.value, data.isPercentage);
        }
        return true;
    }
}

public interface IDestroyableItem
{

}

public interface IItemAction
{
    public string ActionName { get; }
    public AudioClip actionSFX { get; }
    bool PerformAction(GameObject player, List<ItemParameter> itemParameters);
}

[Serializable]
public class ModifierData
{
    public PlayerStatModifierSO statModifier;
    public StatType statType;
    public float value;
    public bool isPercentage;
}
