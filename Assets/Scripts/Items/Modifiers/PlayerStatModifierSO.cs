using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatModifierSO", menuName = "Scriptable Objects/PlayerStatModifierSO")]
public abstract class PlayerStatModifierSO : ScriptableObject
{
    public abstract void AffectPlayer(GameObject player, float val);
}
