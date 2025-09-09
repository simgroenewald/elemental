using UnityEngine;

[CreateAssetMenu(fileName = "HealthLevelModifierSO", menuName = "Stat Modifiers/HealthLevelModifierSO")]
public class HealthLevelModifierSO : ScriptableObject
{
    public void AffectPlayer(GameObject player, string name, float trigger, float duration, float increase, bool isPercentage)
    {
        StatModifierEvents statModifierEvents = player.GetComponent<StatModifierEvents>();
        statModifierEvents.RaiseAddHealthLevelItemEvent(name, trigger, duration, increase, isPercentage);
    }

    public void UnaffectPlayer(GameObject player, string name)
    {
        StatModifierEvents statModifierEvents = player.GetComponent<StatModifierEvents>();
        statModifierEvents.RaiseRemoveHealthLevelItemEvent(name);
    }
}
