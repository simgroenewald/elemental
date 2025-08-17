using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDetailsSO", menuName = "Scriptable Objects/CharacterDetailsSO")]
public class CharacterDetailSO : ScriptableObject
{
    [Header("Character Details")]
    [SerializeField] public string characterName;
    [SerializeField] public GameObject characterPrefab;
    [SerializeField] public RuntimeAnimatorController runtimeAnimatorController;
    [SerializeField] public Sprite miniMapIcon;

    [Header("Character Stats")]
    [SerializeField] public int health;

    [Header("Character Abilities")]
    public AbilityDetailsSO startingAbility;
    public List<AbilityDetailsSO> abilityList;
}
