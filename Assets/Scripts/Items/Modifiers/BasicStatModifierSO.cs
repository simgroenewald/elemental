using UnityEngine;

[CreateAssetMenu(fileName = "BasicStatModifierSO", menuName = "Stat Modifiers/BasicStatModifierSO")]
public class BasicStatModifierSO : PlayerStatModifierSO
{
    public override void AffectPlayer(GameObject player, StatType statType, float val, bool isPercentage)
    {
        StatModifierEvents statModifierEvents = player.GetComponent<StatModifierEvents>();
        statModifierEvents.RaiseAddBasicStatEvent(statType, val, isPercentage);
    }

    public override void UnaffectPlayer(GameObject player, StatType statType, float val, bool isPercentage)
    {
        StatModifierEvents statModifierEvents = player.GetComponent<StatModifierEvents>();
        statModifierEvents.RaiseRemoveBasicStatEvent(statType, val, isPercentage);
    }
}
