using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemSO : ScriptableObject
{
    [field: SerializeField]
    public bool IsStackable { get; set; }
    public int ID => GetInstanceID();

    [field: SerializeField]
    public int MaxStackSize { get; set; }

    [field: SerializeField]
    public string Name { get; set;}

    [TextArea]
    [field: SerializeField]
    public string Description { get; set; }

    [field: SerializeField]
    public Sprite ItemImage { get; set; }

    [field: SerializeField]
    public List<ItemParameter> DefaultParametersList { get; set; }

    [field: SerializeField]
    public int DropChance { get; set; }

    [field: SerializeField]
    public Item ItemPrefab { get; set; }
}

[Serializable]

public struct ItemParameter: IEquatable<ItemParameter>
{
    public ItemParameterSO itemParameterSO;
    public float value;

    public bool Equals(ItemParameter other)
    {
        return other.itemParameterSO == itemParameterSO;
    }
}
