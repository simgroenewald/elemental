using DarkPixelRPGUI.Scripts.UI.Equipment;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

[CreateAssetMenu(fileName = "BackpackSO", menuName = "Scriptable Objects/BackpackSO")]
public class BackpackSO : ScriptableObject
{
    [SerializeField]
    private List<BackpackItem> backpackItems;

    [field: SerializeField]
    public int Size { get; private set; } = 10;

    public event Action<Dictionary<int, BackpackItem>> OnBackpackUpdated;

    public void Initialise()
    {
        backpackItems = new List<BackpackItem>();
        for (int i = 0; i < Size; i++)
        {
            backpackItems.Add(BackpackItem.GetEmptyItem());
        }
    }

    public void AddItem(ItemSO item, int quantity)
    {
        for (int i = 0; i < backpackItems.Count; i++)
        {
            if (backpackItems[i].isEmpty)
            {
                backpackItems[i] = new BackpackItem
                {
                    item = item, 
                    quantity = quantity
                };
                return;
            }
        }
    }

    public Dictionary<int, BackpackItem> GetCurrentBackpackState()
    {
        Dictionary<int, BackpackItem> backpackDict = new Dictionary<int, BackpackItem>();
        for (int i = 0; i < backpackItems.Count; i++)
        {
            if (backpackItems[i].isEmpty)
            {
                continue;
            }
            backpackDict[i] = backpackItems[i];
        }
        return backpackDict;
    }

    public BackpackItem GetItemAtIndex(int index)
    {
        return backpackItems[index];
    }

    public void AddItem(BackpackItem item)
    {
        AddItem(item.item, item.quantity);
    }

    public void SwapItems(int index1, int index2)
    {
        BackpackItem item1 = backpackItems[index1];
        backpackItems[index1] = backpackItems[index2];
        backpackItems[index2] = item1;
        PublicBackpackUpdated();
    }

    private void PublicBackpackUpdated()
    {
        OnBackpackUpdated?.Invoke(GetCurrentBackpackState());
    }
}

[Serializable]
public struct BackpackItem
{
    public int quantity;
    public ItemSO item;
    public bool isEmpty => item == null;

    public BackpackItem UpdateQuantity(int newQuantity)
    {
        return new BackpackItem { 
            item = this.item,
            quantity = newQuantity 
        };
    }

    public static BackpackItem GetEmptyItem() => new BackpackItem
    {
        item = null,
        quantity = 0,
    };
}
