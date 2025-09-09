using UnityEngine;

[CreateAssetMenu(fileName = "AttackCountModifierSO", menuName = "Stat Modifiers/AttackCountModifierSO")]
public class AttackCountModifierSO : ScriptableObject
{
    public void AffectPlayer(GameObject player, StatType statType, string name, float count, float duration, float val, bool isPercentage)
    {
        StatModifierEvents statModifierEvents = player.GetComponent<StatModifierEvents>();
        statModifierEvents.RaiseAddAttackCountItemEvent(statType, name, count, duration, val, isPercentage);
    }

    public void UnaffectPlayer(GameObject player, string name)
    {
        StatModifierEvents statModifierEvents = player.GetComponent<StatModifierEvents>();
        statModifierEvents.RaiseRemoveAttackCountItemEvent(name);
    }
}
