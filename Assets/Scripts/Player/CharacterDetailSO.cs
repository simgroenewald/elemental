using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDetailsSO", menuName = "Scriptable Objects/CharacterDetailsSO")]
public class CharacterDetailSO : ScriptableObject
{
    [SerializeField] public string characterName;
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public RuntimeAnimatorController runtimeAnimatorController;
    [SerializeField] public int health;
    [SerializeField] public Sprite miniMapIcon;
}
