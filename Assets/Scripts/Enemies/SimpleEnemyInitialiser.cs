using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class SimpleEnemyInitialiser : MonoBehaviour
{
    [Header("Enemy Initialisation")]
    [SerializeField] private List<GameObject> enemies;

    public List<GameObject> GetEnemyPrefabs()
    {

        return enemies;
    }
}