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

    [field: SerializeField]
    [TextArea]
    public string Description { get; set; }

    [field: SerializeField]
    public Sprite ItemImage { get; set; }
}
