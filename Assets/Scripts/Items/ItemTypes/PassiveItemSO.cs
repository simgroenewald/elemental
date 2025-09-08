using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItemSO", menuName = "Item Types/PassiveItemSO")]
public class PassiveItemSO : ItemSO, IDroppable, IItemPassive
{
    [SerializeField]
    private List<ModifierData> modifiersData = new List<ModifierData>();

    public AudioClip actionSFX {get; private set;}

    public bool ApplyEffects(GameObject player, List<ItemParameter> itemParameters = null)
    {
        foreach (ModifierData data in modifiersData)
        {
            data.statModifier.AffectPlayer(player, data.statType, data.value, data.isPercentage);
        }
        return true;
    }

    public bool RemoveEffects(GameObject player, List<ItemParameter> itemParameters = null)
    {
        foreach (ModifierData data in modifiersData)
        {
            data.statModifier.UnaffectPlayer(player, data.statType, data.value, data.isPercentage);
        }
        return true;
    }
}

public interface IDroppable
{
    bool RemoveEffects(GameObject player, List<ItemParameter> itemParameters);
}

public interface IItemPassive
{
    public AudioClip actionSFX { get; }
    bool ApplyEffects(GameObject player, List<ItemParameter> itemParameters);
    bool RemoveEffects(GameObject player, List<ItemParameter> itemParameters);
}
