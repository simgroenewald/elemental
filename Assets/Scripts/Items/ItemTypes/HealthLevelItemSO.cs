using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthLevelItemSO", menuName = "Item Types/HealthLevelItemSO")]
public class HealthLevelItemSO : ItemSO, IDroppable, IItemPassive
{
    [SerializeField]
    private List<HealthLevelModifierData> healthLevelModifierData = new List<HealthLevelModifierData>();

    public AudioClip actionSFX {get; private set;}

    public bool ApplyEffects(GameObject player, List<ItemParameter> itemParameters = null)
    {
        foreach (HealthLevelModifierData data in healthLevelModifierData)
        {
            data.statModifier.AffectPlayer(player, data.name, data.trigger, data.duration, data.increase, data.isPercentage);
        }
        return true;
    }

    public bool RemoveEffects(GameObject player, List<ItemParameter> itemParameters = null)
    {
        foreach (HealthLevelModifierData data in healthLevelModifierData)
        {
            data.statModifier.UnaffectPlayer(player, data.name);
        }
        return true;
    }
}

[Serializable]
public class HealthLevelModifierData
{
    public HealthLevelModifierSO statModifier;
    public string name;
    public float trigger;
    public float duration;
    public float increase;
    public bool isPercentage;
}
