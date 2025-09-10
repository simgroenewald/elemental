using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class SimpleEnemyInitialiser : MonoBehaviour
{

    [SerializeField] private EnemyRoomTypeMapperSO mapper;


    public void Awake()
    {
        mapper.PopulateEnemiesDicts();
    }

    public List<GameObject> GetEnemyPrefabs(ElementTheme theme)
    {
        List<GameObject> enemies = mapper.themeToEnemiesDict[theme];
        return enemies;
    }
    public GameObject GetMiniBossPrefab(ElementTheme theme)
    {
        List<GameObject> miniBosses = mapper.themeToMiniBossesDict[theme];
        GameObject miniBoss = miniBosses[Random.Range(0, miniBosses.Count)];
        return miniBoss;
    }
    public GameObject GetBossPrefab(ElementTheme theme)
    {
        List<GameObject> bosses = mapper.themeToBossesDict[theme];
        GameObject boss = bosses[Random.Range(0, bosses.Count)];
        return boss;
    }

}