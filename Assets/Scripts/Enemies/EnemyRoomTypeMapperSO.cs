using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyRoomTypeMapperSO", menuName = "Scriptable Objects/EnemyRoomTypeMapperSO")]
public class EnemyRoomTypeMapperSO : ScriptableObject
{
    public List<GameObject> earthEnemies;
    public List<GameObject> waterEnemies;
    public List<GameObject> fireEnemies;
    public List<GameObject> airEnemies;
    public Dictionary<ElementTheme, List<GameObject>> themeToEnemiesDict = new Dictionary<ElementTheme, List<GameObject>>();

    public List<GameObject> earthMiniBosses;
    public List<GameObject> waterMiniBosses;
    public List<GameObject> fireMiniBosses;
    public List<GameObject> airMiniBosses;
    public Dictionary<ElementTheme, List<GameObject>> themeToMiniBossesDict = new Dictionary<ElementTheme, List<GameObject>>();

    public void PopulateEnemiesDicts()
    {
        themeToEnemiesDict[ElementTheme.Earth] = earthEnemies;
        themeToEnemiesDict[ElementTheme.Water] = waterEnemies;
        themeToEnemiesDict[ElementTheme.Fire] = fireEnemies;
        themeToEnemiesDict[ElementTheme.Air] = airEnemies;

        themeToMiniBossesDict[ElementTheme.Earth] = earthMiniBosses;
        themeToMiniBossesDict[ElementTheme.Water] = waterMiniBosses;
        themeToMiniBossesDict[ElementTheme.Fire] = fireMiniBosses;
        themeToMiniBossesDict[ElementTheme.Air] = airMiniBosses;
    }
}
