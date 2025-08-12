using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class SimpleEnemyInitializer : MonoBehaviour
{
    [Header("Enemy Initialization")]
    [SerializeField] private GameObject porcumanBluePrefab;

    public GameObject GetEnemyPrefab()
    {

        return porcumanBluePrefab;
    }
}