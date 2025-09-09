using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemEvents", menuName = "Events/ItemEvents")]
public class ItemEvents : ScriptableObject
{
    public event Action<DungeonRoom, Transform> OnItemDrop;
    public void RaiseDropItemEvent(DungeonRoom room, Transform transform)
    {
        OnItemDrop?.Invoke(room, transform);
    }
}
