using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemEvents", menuName = "Events/ItemEvents")]
public class ItemEvents : ScriptableObject
{
    public event Action<DungeonRoom, int, Transform> OnItemDrop;
    public void RaiseDropItemEvent(DungeonRoom room, int health, Transform transform)
    {
        OnItemDrop?.Invoke(room, health, transform);
    }
}
