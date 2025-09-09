using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableStatModifierSO", menuName = "Stat Modifiers/ConsumableStatModifierSO")]
public class ConsumableStatModifierSO : PlayerStatModifierSO
{
    public override void AffectPlayer(GameObject player, StatType statType, float val, bool isPercentage)
    {
        StatModifierEvents statModifierEvents = player.GetComponent<StatModifierEvents>();
        statModifierEvents.RaiseConsumableUsedEvent(statType, val, isPercentage);
    }

    public override void UnaffectPlayer(GameObject player, StatType statType, float val, bool isPercentage)
    {
    }
}
