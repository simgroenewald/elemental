using DarkPixelRPGUI.Scripts.UI.Equipment;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
    public event Action<int> OnNewItemAdded;

    public void Initialise()
    {
        backpackItems = new List<BackpackItem>();
        for (int i = 0; i < Size; i++)
        {
            backpackItems.Add(BackpackItem.GetEmptyItem());
        }
    }

    // UPDATE THIS
    public int AddItem(ItemSO item, int quantity, List<ItemParameter> itemParameters = null)
    {
        if (item.IsStackable == false)
        {
            for (int i = 0; i < backpackItems.Count; i++)
            {
                while (quantity > 0 && IsInventoryFull() == false)
                {
                    quantity = quantity - AddItemToNewSlot(item, 1, itemParameters);
                }
                PublishBackpackUpdated();
                return quantity;
            }
        }
        quantity = StackItemInExistingSlot(item, quantity);
        PublishBackpackUpdated();
        return quantity;
    }

    private int AddItemToNewSlot(ItemSO item, int quantity, List<ItemParameter> itemParameters = null)
    {
        BackpackItem newBackpackItem = new BackpackItem
        {
            item = item,
            quantity = quantity,
            itemParameters = new List<ItemParameter>(itemParameters == null ? item.DefaultParametersList : itemParameters)
        };
        for (int i = 0;i < backpackItems.Count;i++)
        {
            if (backpackItems[i].isEmpty)
            {
                backpackItems[i] = newBackpackItem;
                OnNewItemAdded?.Invoke(i);
                return quantity;
            }
        }
        return 0;

    }

    private bool IsInventoryFull() => backpackItems.Where(item => item.isEmpty).Any() == false;

    private int StackItemInExistingSlot(ItemSO item, int quantity)
    {
        for (int i = 0; i < backpackItems.Count; i++)
        {
            if (backpackItems[i].isEmpty)
                continue;
            if (backpackItems[i].item.ID == item.ID)
            {
                int amountAllowable = backpackItems[i].item.MaxStackSize - backpackItems[i].quantity;

                if (quantity > amountAllowable)
                {
                    backpackItems[i] = backpackItems[i].UpdateQuantity(backpackItems[i].item.MaxStackSize);
                    quantity -= amountAllowable;
                } else
                {
                    backpackItems[i] = backpackItems[i].UpdateQuantity(backpackItems[i].quantity + quantity);
                    return 0;
                }
            }
        }
        while (quantity > 0 && IsInventoryFull() == false)
        {
            int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
            quantity -= newQuantity;
            AddItemToNewSlot(item, newQuantity);
        }
        return quantity;
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

    public List<BackpackItem> GetAllItems()
    {
        return backpackItems;
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
        PublishBackpackUpdated();
    }

    private void PublishBackpackUpdated()
    {
        OnBackpackUpdated?.Invoke(GetCurrentBackpackState());
    }

    public void RemoveItem(int index, int quantity)
    {
        if (backpackItems.Count > index)
        {
            if (backpackItems[index].isEmpty)
                return;
            int remainder = backpackItems[index].quantity - quantity;
            if (remainder <= 0)
            {
                backpackItems[index] = BackpackItem.GetEmptyItem();
            }
            else
            {
                backpackItems[index] = backpackItems[index].UpdateQuantity(remainder);
            }

            PublishBackpackUpdated();
        }
    }
}

[Serializable]
public struct BackpackItem
{
    public int quantity;
    public ItemSO item;
    public List<ItemParameter> itemParameters;
    public bool isEmpty => item == null;

    public BackpackItem UpdateQuantity(int newQuantity)
    {
        return new BackpackItem {
            item = this.item,
            quantity = newQuantity,
            itemParameters = new List<ItemParameter>(this.itemParameters)
        };
    }

    public static BackpackItem GetEmptyItem() => new BackpackItem
    {
        item = null,
        quantity = 0,
        itemParameters = new List<ItemParameter>()
    };
}
