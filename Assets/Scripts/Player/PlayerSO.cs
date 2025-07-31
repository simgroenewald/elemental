using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "Scriptable Objects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    public CharacterDetailSO characterDetails;
    public string playerName;
}
