using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackCountItemSO", menuName = "Item Types/AttackCountItemSO")]
public class AttackCountItemSO : ItemSO, IDestroyableItem, IItemPassive
{
    [SerializeField]
    private List<AttackCountModifierData> attackCountModifierData = new List<AttackCountModifierData>();

    public AudioClip actionSFX {get; private set;}

    public bool ApplyEffects(GameObject player, List<ItemParameter> itemParameters = null)
    {
        foreach (AttackCountModifierData data in attackCountModifierData)
        {
            data.statModifier.AffectPlayer(player, data.statType, data.count, data.duration, data.val, data.isPercentage);
        }
        return true;
    }

    public bool RemoveEffects(GameObject player, List<ItemParameter> itemParameters = null)
    {
        foreach (AttackCountModifierData data in attackCountModifierData)
        {
            data.statModifier.UnaffectPlayer(player, data.statType, data.count, data.duration, data.val, data.isPercentage);
        }
        return true;
    }
}

[Serializable]
public class AttackCountModifierData
{
    public AttackCountModifierSO statModifier;
    public StatType statType;
    public float count;
    public float duration;
    public float val;
    public bool isPercentage;
}
