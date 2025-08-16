using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetailsSO", menuName = "Scriptable Objects/EnemyDetailsSO")]
public class EnemyDetailsSO : ScriptableObject
{
    public string enemyName;
    public GameObject enemyPrefab;

    public float moveSpeed;

    [Header("Enemy Abilities")]
    public AbilityDetailsSO startingAbility;

    public List<AbilityDetailsSO> abilityList;
}
