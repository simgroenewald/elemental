using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatModifierSO", menuName = "Scriptable Objects/PlayerStatModifierSO")]
public abstract class PlayerStatModifierSO : ScriptableObject
{
    public abstract void AffectPlayer(GameObject player, StatType statType, float val, bool isPercentage);
    public abstract void UnaffectPlayer(GameObject player, StatType statType, float val, bool isPercentage);
}
