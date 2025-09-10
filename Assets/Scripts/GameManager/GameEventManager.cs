using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance { get; private set; }

    public TargetEvents targetEvents;
    public ItemEvents itemEvents;
    public GlobalUIEvents globalUIEvents;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }
}