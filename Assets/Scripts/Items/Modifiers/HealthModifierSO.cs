using UnityEngine;

[CreateAssetMenu(fileName = "HealthModifierSO", menuName = "Scriptable Objects/HealthModifierSO")]
public class HealthModifierSO : PlayerStatModifierSO
{
    public override void AffectPlayer(GameObject player, float val)
    {
        HealthEvents healthEvents = player.GetComponent<HealthEvents>();
        healthEvents.RaiseIncreaseHealthEvent(val);
    }
}
