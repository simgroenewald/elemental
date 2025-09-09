using GLTFast.Schema;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> globalItems;
    [SerializeField] private ElementToItemsListMapperSO elementToBasicItemsListMapperSO;
    [SerializeField] private ElementToItemsListMapperSO elementToUltraItemsListMapperSO;
    private Dictionary<ElementTheme, List<GameObject>> elementToBasicItemsListDict;
    private Dictionary<ElementTheme, List<GameObject>> elementToUltraItemsListDict;


    private void Start()
    {
        GameEventManager.Instance.itemEvents.OnItemDrop += SpawnItem;

        elementToBasicItemsListMapperSO.Initialise();
        elementToBasicItemsListDict = elementToBasicItemsListMapperSO.GetElementToItemsListDict();

        elementToUltraItemsListMapperSO.Initialise();
        elementToUltraItemsListDict = elementToUltraItemsListMapperSO.GetElementToItemsListDict();
    }

    private void OnDisable()
    {
        GameEventManager.Instance.itemEvents.OnItemDrop -= SpawnItem;
    }


    private GameObject GetItemToDrop(DungeonRoom room)
    {
        Debug.Log("Drop Item");

        int basicItemDrop = UnityEngine.Random.Range(1, 101);

        int chanceToBeat;
        if (room.isComplete) 
            chanceToBeat = 40;
        else 
            chanceToBeat = 20;

        // 20% chance to drop a basic item
        if (basicItemDrop < chanceToBeat)
        {
            GameObject basicItem = GetItemFromDropChances(elementToBasicItemsListDict[room.theme]);
            if (basicItem != null)
            {
                return basicItem;
            }
        }

        GameObject globalItem = GetItemFromDropChances(globalItems);
        return globalItem;
    }

    private GameObject GetItemFromDropChances(List<GameObject> itemList)
    {
        int basicItemPercentage = UnityEngine.Random.Range(1, 101);
        List<GameObject> possibleItems = new List<GameObject>();
        foreach (var itemObject in itemList)
        {
            Item item = itemObject.GetComponent<Item>();
            if (basicItemPercentage <= item.ItemSO.DropChance)
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

    private void SpawnItem(DungeonRoom room, Transform transform)
    {
        GameObject itemObjectToDrop = GetItemToDrop(room);

        if (itemObjectToDrop == null)
            return;

        Item item = itemObjectToDrop.GetComponent<Item>();
        Instantiate(itemObjectToDrop, transform.position, Quaternion.identity, room.transform);
        itemObjectToDrop.name = item.name;
        // Ensure proper 2D rotation (no X or Y rotation)
        itemObjectToDrop.transform.rotation = Quaternion.identity;
    }

    private GameObject GetMinibossItemChoices(int itemCoiceCount, DungeonRoom room)
    {
        List<GameObject> itemChoices = new List<GameObject>();
        BackpackSO backpack = GameManager.Instance.player.GetComponent<BackpackController>().backpack;

        while (itemChoices.Count < itemCoiceCount)
        {
            int ultraItemDrop = UnityEngine.Random.Range(1, 101);

            // 20% chance to drop a basic item
            if (ultraItemDrop < 20)
            {
                GameObject ultraItem = elementToUltraItemsListDict[room.theme][UnityEngine.Random.Range(0, elementToUltraItemsListDict[room.theme].Count)];
                bool containsUltraItem = BackPackContainsItem(backpack, ultraItem);
                if (!containsUltraItem)
                {
                    itemChoices.Add(ultraItem);
                    continue;
                }
            }
            GameObject basicItem = GetItemFromDropChances(elementToBasicItemsListDict[room.theme]);
            bool containsItem = BackPackContainsItem(backpack, basicItem);
            if (!containsItem)
            {
                itemChoices.Add(basicItem);
            }
        }

        GameObject globalItem = GetItemFromDropChances(globalItems);
        return globalItem;
    }

    private GameObject GetBossItemChoices(int itemCoiceCount, DungeonRoom room)
    {
        List<GameObject> itemChoices = new List<GameObject>();
        BackpackSO backpack = GameManager.Instance.player.GetComponent<BackpackController>().backpack;

        GameObject ultraItem = elementToUltraItemsListDict[room.theme][UnityEngine.Random.Range(0, elementToUltraItemsListDict[room.theme].Count)];
        bool containsUltraItem = BackPackContainsItem(backpack, ultraItem);
        if (!containsUltraItem)
        {
            itemChoices.Add(ultraItem);
        }

        while (itemChoices.Count < itemCoiceCount)
        {
            GameObject basicItem = GetItemFromDropChances(elementToBasicItemsListDict[room.theme]);
            bool containsItem = BackPackContainsItem(backpack, basicItem);
            if (!containsItem)
            {
                itemChoices.Add(basicItem);
            }
        }

        GameObject globalItem = GetItemFromDropChances(globalItems);
        return globalItem;
    }

    private bool BackPackContainsItem(BackpackSO backpack, GameObject basicItem)
    {
        if (basicItem == null) return false;
        foreach (var backpackitem in backpack.GetAllItems())
        {
            if (backpackitem.item.ID == basicItem.GetComponent<ItemSO>().ID)
            {
                return true;
            }
        }
        return false;
    }
}
