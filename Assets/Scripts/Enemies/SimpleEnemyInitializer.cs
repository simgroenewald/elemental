using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class SimpleEnemyInitializer : MonoBehaviour
{
    [Header("Enemy Initialization")]
    [SerializeField] private List<GameObject> enemies;

    public List<GameObject> GetEnemyPrefabs()
    {

        return enemies;
    }
}