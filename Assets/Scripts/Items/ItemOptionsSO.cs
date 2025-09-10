using DarkPixelRPGUI.Scripts.UI.Equipment;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEditorInternal;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemOptionsSO", menuName = "Scriptable Objects/ItemOptionsSO")]
public class ItemOptionsSO : ScriptableObject
{
    [SerializeField]
    private List<ItemSO> itemOptions;

    [field: SerializeField]
    public int Size { get; private set; } = 3;

    public event Action<Dictionary<int, ItemSO>> OnItemOptionsUpdated;


    public void AddItems(List<ItemSO> items)
    {
        itemOptions = new List<ItemSO>();
        for (int i = 0; i < items.Count; i++)
        {
            itemOptions.Add(items[i]);
        }    
        PublishItemOptionsUpdated();
    }

    public ItemSO GetItemAtIndex(int index)
    {
        return itemOptions[index];
    }

    public List<ItemSO> GetAllItems()
    {
        return itemOptions;
    }

    private void PublishItemOptionsUpdated()
    {
        Dictionary<int, ItemSO> currentItemOptions = GetCurrentItemOptionsState();
        OnItemOptionsUpdated?.Invoke(currentItemOptions);
    }

    public Dictionary<int, ItemSO> GetCurrentItemOptionsState()
    {
        Dictionary<int, ItemSO> itemOptionsDict = new Dictionary<int, ItemSO>();
        for (int i = 0; i < itemOptions.Count; i++)
        {
            itemOptionsDict[i] = itemOptions[i];
        }
        return itemOptionsDict;
    }
}
