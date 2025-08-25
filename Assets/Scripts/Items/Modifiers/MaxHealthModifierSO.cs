using UnityEngine;

[CreateAssetMenu(fileName = "MaxHealthModifierSO", menuName = "Scriptable Objects/MaxHealthModifierSO")]
public class MaxHealthModifierSO : PlayerStatModifierSO
{
    public override void AffectPlayer(GameObject player, float val)
    {
        StatModifierEvents statModifierEvent = player.GetComponent<StatModifierEvents>();
        statModifierEvent.RaiseIncreaseMaxHealth(val);
    }
}
