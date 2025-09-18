using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetailsSO", menuName = "Scriptable Objects/EnemyDetailsSO")]
public class EnemyDetailsSO : CharacterDetailSO
{
    public bool isBoss;
    public bool isMiniBoss;
    public List<StatsSO> statsSO;
}
