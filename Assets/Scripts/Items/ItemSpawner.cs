using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    List<GameObject> waterRoomItems;
    [SerializeField]
    List<GameObject> fireRoomItems;
    [SerializeField]
    List<GameObject> airRoomItems;
    [SerializeField]
    List<GameObject> earthRoomItems;

    Dictionary<ElementTheme, List<GameObject>> themeToEnemiesDict = new Dictionary<ElementTheme, List<GameObject>>();
    private void Start()
    {
        GameEventManager.Instance.itemEvents.OnItemDrop += SpawnItem;

        themeToEnemiesDict[ElementTheme.Water] = waterRoomItems;
        themeToEnemiesDict[ElementTheme.Fire] = fireRoomItems;
        themeToEnemiesDict[ElementTheme.Air] = airRoomItems;
        themeToEnemiesDict[ElementTheme.Earth] = earthRoomItems;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.itemEvents.OnItemDrop -= SpawnItem;
    }


    private GameObject GetItemToDrop(ElementTheme theme, int health)
    {
        Debug.Log("Drop Item");
        int rndNumber = UnityEngine.Random.Range(1, 101);
        List<GameObject> possibleItems = new List<GameObject>();

        foreach (var itemObject in themeToEnemiesDict[theme])
        {
            Item item = itemObject.GetComponent<Item>();
            if (rndNumber <= item.ItemSO.DropChance)
            {
                possibleItems.Add(itemObject);
            }
        }
        if (possibleItems.Count > 0)
        {
            GameObject itemToDrop = possibleItems[UnityEngine.Random.Range(0, possibleItems.Count)];
            return itemToDrop;
        }
        return null;
    }

    private void SpawnItem(DungeonRoom room, int health, Transform transform)
    {
        GameObject itemObjectToDrop = GetItemToDrop(room.theme, health);

        Item item = itemObjectToDrop.GetComponent<Item>();
        Instantiate(itemObjectToDrop, transform.position, Quaternion.identity, room.transform);
        itemObjectToDrop.name = item.name;
        // Ensure proper 2D rotation (no X or Y rotation)
        itemObjectToDrop.transform.rotation = Quaternion.identity;
    }
}
