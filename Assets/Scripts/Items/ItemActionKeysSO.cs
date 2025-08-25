using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemActionKeys", menuName = "Scriptable Objects/ItemActionKeys")]
public class ItemActionKeysSO : ScriptableObject
{
    [field: SerializeField]
    public List<ItemActionKeyBinding> itemActionKeyBindings { get; set; }
}

[Serializable]

public struct ItemActionKeyBinding
{
    public KeyCode key;
    public int index;
}
