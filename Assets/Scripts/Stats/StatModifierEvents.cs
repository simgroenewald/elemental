using System;
using UnityEngine;

public class StatModifierEvents : MonoBehaviour
{
    public event Action<float, bool> OnModifyMaxHealthEvent;
    public event Action<float, bool> OnModifyMaxManaEvent;
    public event Action<StatType, float, bool> OnAddBasicStatEvent;
    public event Action<StatType, float, bool> OnRemoveBasicStatEvent;
    public event Action<string, float, float, float, bool> OnAddHealthLevelItemEvent;
    public event Action<string> OnRemoveHealthLevelItemEvent;
    public event Action<StatType, string, float, float, float, bool> OnAddAttackCountItemEvent;
    public event Action<string> OnRemoveAttackCountItemEvent;

    public void RaiseModifyMaxHealthEvent(float val, bool isPercentage)
    {
        OnModifyMaxHealthEvent?.Invoke(val, isPercentage);
    }

    public void RaiseModifyMaxManaEvent(float val, bool isPercentage)
    {
        OnModifyMaxManaEvent?.Invoke(val, isPercentage);
    }

    internal void RaiseAddHealthLevelItemEvent(string name, float trigger, float duration, float increase, bool isPercentage)
    {
        Debug.Log($"Modify health level ({name}) at {trigger} by {increase} for {duration} seconds");
        OnAddHealthLevelItemEvent?.Invoke(name, trigger, duration, increase, isPercentage);
    }

    internal void RaiseAddBasicStatEvent(StatType statType, float val, bool isPercentage)
    {
        Debug.Log($"Modify Basic stat {statType} by {val}");
        OnAddBasicStatEvent?.Invoke(statType, val, isPercentage);
    }

    internal void RaiseRemoveBasicStatEvent(StatType statType, float val, bool isPercentage)
    {
        Debug.Log($"Modify Basic stat {statType} by {val}");
        OnRemoveBasicStatEvent?.Invoke(statType, val, isPercentage);
    }

    internal void RaiseAddAttackCountItemEvent(StatType statType, string name, float count, float duration, float val, bool isPercentage)
    {
        OnAddAttackCountItemEvent?.Invoke(statType, name, count, duration, val, isPercentage);
    }

    internal void RaiseRemoveAttackCountItemEvent(string name)
    {
        OnRemoveAttackCountItemEvent?.Invoke(name);
    }

    internal void RaiseRemoveHealthLevelItemEvent(string name)
    {
        OnRemoveHealthLevelItemEvent?.Invoke(name);
    }
}