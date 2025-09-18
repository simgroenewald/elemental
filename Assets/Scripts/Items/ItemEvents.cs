using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemEvents", menuName = "Events/ItemEvents")]
public class ItemEvents : ScriptableObject
{
    public Action<DungeonRoom> OnShowMinibossItems { get; internal set; }
    public Action<DungeonRoom> OnShowBossItems { get; internal set; }

    public event Action<DungeonRoom, Transform> OnItemDrop;

    public event Action<DungeonRoom, Item> OnSelectedItemDrop;
    public void RaiseDropItemEvent(DungeonRoom room, Transform transform)
    {
        OnItemDrop?.Invoke(room, transform);
    }

    public void RaiseDropSelectedItemEvent(DungeonRoom room, Item item)
    {
        OnSelectedItemDrop?.Invoke(room, item);
    }

    public void RaiseShowMinibossItemsEvent(DungeonRoom room)
    {
        OnShowMinibossItems?.Invoke(room);
    }

    public void RaiseShowBossItemsEvent(DungeonRoom room)
    {
        OnShowBossItems?.Invoke(room);
    }
}
