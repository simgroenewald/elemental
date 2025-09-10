using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalUIEvents", menuName = "Events/GlobalUIEvents")]
public class GlobalUIEvents : ScriptableObject
{
    public event Action<Character> OnShowBossMainHealthBar;
    public event Action OnRemoveBossMainHealthBar;
    public void RaiseShowBossMainHealthBarEvent(Character enemy)
    {
        OnShowBossMainHealthBar?.Invoke(enemy);
    }

    public void RaiseRemoveBossMainHealthBarEvent()
    {
        OnRemoveBossMainHealthBar?.Invoke();
    }
}
