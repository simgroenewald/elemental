using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetailsSO", menuName = "Scriptable Objects/EnemyDetailsSO")]
public class EnemyDetailsSO : ScriptableObject
{
    public string enemyName;
    public GameObject enemyPrefab;
}
