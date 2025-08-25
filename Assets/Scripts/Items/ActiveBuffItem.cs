using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActiveBuffItem", menuName = "Scriptable Objects/ActiveBuffItem")]
public class ActiveBuffItem : ItemSO, IDestroyableItem, IItemAction
{
    public string ActionName => "Equip";

    public AudioClip actionSFX { get; private set; }

    public bool PerformAction(GameObject player, List<ItemParameter> itemParameters = null)
    {
        ItemController itemController = player.GetComponent<ItemController>();
        if (itemController != null)
        {
            itemController.SetItem(this, itemParameters == null ? DefaultParametersList : itemParameters);
            return true;
        }
        return false;
    }
}
