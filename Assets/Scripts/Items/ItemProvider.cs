using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemProvider : MonoBehaviour
{
    [SerializeField] List<GameObject> globalItems;
    [SerializeField] List<ItemDropChance> itemDropChances;
    [SerializeField] SoundEffectSO itemDropSound;
    [SerializeField] private ElementToItemsListMapperSO elementToBasicItemsListMapperSO;
    [SerializeField] private ElementToItemsListMapperSO elementToUltraItemsListMapperSO;
    private Dictionary<ElementTheme, List<GameObject>> elementToBasicItemsListDict;
    private Dictionary<ElementTheme, List<GameObject>> elementToUltraItemsListDict;
    private Player player;
    private BackpackSO backpack;



    private void Start()
    {
        GameEventManager.Instance.itemEvents.OnItemDrop += SpawnItem;
        GameEventManager.Instance.itemEvents.OnSelectedItemDrop += SpawnSelectedItem;
        GameEventManager.Instance.itemEvents.OnShowMinibossItems += ShowMinibossItemChoices;
        GameEventManager.Instance.itemEvents.OnShowBossItems += ShowBossItemChoices;

        elementToBasicItemsListMapperSO.Initialise();
        elementToBasicItemsListDict = elementToBasicItemsListMapperSO.GetElementToItemsListDict();

        elementToUltraItemsListMapperSO.Initialise();
        elementToUltraItemsListDict = elementToUltraItemsListMapperSO.GetElementToItemsListDict();

        player = GameManager.Instance.player;
        backpack = player.backpackController.backpack;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.itemEvents.OnItemDrop -= SpawnItem;
        GameEventManager.Instance.itemEvents.OnSelectedItemDrop -= SpawnSelectedItem;
        GameEventManager.Instance.itemEvents.OnShowMinibossItems -= ShowMinibossItemChoices;
        GameEventManager.Instance.itemEvents.OnShowBossItems -= ShowBossItemChoices;
    }


    private GameObject GetItemToDrop(DungeonRoom room)
    {
        System.Random dropRandom = new System.Random();

        int basicItemDrop = dropRandom.Next(1, 101); ;

        int chanceToBeat;
        if (room.isComplete) 
            chanceToBeat = itemDropChances[GameManager.Instance.currentLevelIndex].normalRoomCompleteItemDropChance;
        else 
            chanceToBeat = itemDropChances[GameManager.Instance.currentLevelIndex].normalRoomItemDropChance;

        // 20% chance to drop a basic item
        if (basicItemDrop < chanceToBeat)
        {
            GameObject basicItem = GetItemFromDropChances(elementToBasicItemsListDict[room.theme]);
            if (basicItem != null)
            {
                return basicItem;
            }
        }

        if (basicItemDrop < itemDropChances[GameManager.Instance.currentLevelIndex].normalRoomConsumableDropChance)
        {
            GameObject globalItem = GetGlobalItemFromDropChances(globalItems);
            return globalItem;
        }

        return null;
    }

    private GameObject GetItemFromDropChances(List<GameObject> itemList)
    {
        System.Random dropRandom = new System.Random();
        int itemPercentage = dropRandom.Next(1, 101);
        List<GameObject> possibleItems = new List<GameObject>();
        foreach (var itemObject in itemList)
        {
            Item item = itemObject.GetComponent<Item>();

            if (itemPercentage <= item.ItemSO.DropChance)
            {
                bool containsBasicItem = backpack.ContainsItem(item.GetComponent<Item>().ItemSO);
                if (!containsBasicItem)
                {
                    possibleItems.Add(itemObject);
                }
            }
        }
        if (possibleItems.Count > 0)
        {
            GameObject itemToDrop = possibleItems[dropRandom.Next(0, possibleItems.Count)];
            return itemToDrop;
        }
        return null;
    }

    private GameObject GetGlobalItemFromDropChances(List<GameObject> itemList)
    {
        System.Random dropRandom = new System.Random();
        int itemPercentage = dropRandom.Next(1, 101);
        List<GameObject> possibleItems = new List<GameObject>();
        foreach (var itemObject in itemList)
        {
            Item item = itemObject.GetComponent<Item>();

            if (itemPercentage <= item.ItemSO.DropChance)
            {
                possibleItems.Add(itemObject);
            }
        }
        if (possibleItems.Count > 0)
        {
            GameObject itemToDrop = possibleItems[dropRandom.Next(0, possibleItems.Count)];
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
        SoundEffectManager.Instance.PlaySoundEffect(itemDropSound);
    }

    public void SpawnSelectedItem(DungeonRoom room, Item itemToDrop)
    {
        if (itemToDrop == null)
            return;

        Item item = itemToDrop.GetComponent<Item>();
        Instantiate(itemToDrop, GameManager.Instance.player.transform.position, Quaternion.identity, room.transform);
        itemToDrop.name = item.name;
        // Ensure proper 2D rotation (no X or Y rotation)
        itemToDrop.transform.rotation = Quaternion.identity;
        SoundEffectManager.Instance.PlaySoundEffect(itemDropSound);
    }

    private void ShowMinibossItemChoices(DungeonRoom room)
    {
        System.Random dropRandom = new System.Random();
        List<ItemSO> itemChoices = new List<ItemSO>();

        while (itemChoices.Count != 3)
        {
            int ultraItemDrop = dropRandom.Next(1, 101);

            // 20% chance to drop a ultra item
            if (ultraItemDrop < itemDropChances[GameManager.Instance.currentLevelIndex].miniBossRoomUltraDropChance)
            {
                GameObject ultraItemGO = elementToUltraItemsListDict[room.theme][dropRandom.Next(0, elementToUltraItemsListDict[room.theme].Count)];
                
                if (!ultraItemGO) continue;
                ItemSO ultraItem = ultraItemGO.GetComponent<Item>().ItemSO;
                bool containsUltraItem = backpack.ContainsItem(ultraItem);
                if (!containsUltraItem && !itemChoices.Contains(ultraItem))
                {
                    itemChoices.Add(ultraItem);
                    continue;
                }
            }
            GameObject basicItemGO = GetItemFromDropChances(elementToBasicItemsListDict[room.theme]);
            if (!basicItemGO) continue;
            ItemSO basicItem = basicItemGO.GetComponent<Item>().ItemSO;
            bool containsItem = backpack.ContainsItem(basicItem);
            if (!containsItem && !itemChoices.Contains(basicItem))
            {
                itemChoices.Add(basicItem);
            }
        }

        player.itemSelectorController.SetUpItemSelection(itemChoices);
    }

    private void ShowBossItemChoices(DungeonRoom room)
    {
        System.Random dropRandom = new System.Random();
        List<ItemSO> itemChoices = new List<ItemSO>();

        GameObject ultraItemGO = elementToUltraItemsListDict[room.theme][dropRandom.Next(0, elementToUltraItemsListDict[room.theme].Count)];
        ItemSO ultraItem = ultraItemGO.GetComponent<Item>().ItemSO;
        bool containsUltraItem = backpack.ContainsItem(ultraItem);
        if (!containsUltraItem)
        {
            itemChoices.Add(ultraItem);
        }

        while (itemChoices.Count != 3)
        {
            GameObject basicItemGO = GetItemFromDropChances(elementToBasicItemsListDict[room.theme]);
            if (!basicItemGO) continue;
            ItemSO basicItem = basicItemGO.GetComponent<Item>().ItemSO;
            bool containsItem = backpack.ContainsItem(basicItem);
            if (!containsItem && !itemChoices.Contains(basicItem))
            {
                itemChoices.Add(basicItem);
            }
        }

        player.itemSelectorController.SetUpItemSelection(itemChoices);
    }
}


[Serializable]
public class ItemDropChance
{
    public int normalRoomConsumableDropChance;
    public int normalRoomItemDropChance;
    public int normalRoomCompleteItemDropChance;
    public int miniBossRoomUltraDropChance;
}
