using System;
using UnityEngine;

public class StatModifierEvents : MonoBehaviour
{
    public event Action<float, bool> OnModifyMaxHealthEvent;
    public event Action<float, bool> OnModifyMaxManaEvent;
    public event Action<StatType, float, bool> OnAddBasicStatEvent;
    public event Action<StatType, float, bool> OnRemoveBasicStatEvent;
    public event Action<string, float, float, float, bool> OnAddHealthLevelItemEvent;

    public void RaiseModifyMaxHealthEvent(float val, bool isPercentage)
    {
        OnModifyMaxHealthEvent?.Invoke(val, isPercentage);
    }

    public void RaiseModifyMaxManaEvent(float val, bool isPercentage)
    {
        OnModifyMaxManaEvent?.Invoke(val, isPercentage);
    }

    internal void RaiseAddAttackCountItemEvent(StatType statType, float count, float duration, float val, bool isPercentage)
    {
        Debug.Log($"Modify attack at {count} attacks by {val} for {duration}");
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

    internal void RaiseRemoveAttackCountItemEvent(StatType statType, float count, float duration, float val, bool isPercentage)
    {
        throw new NotImplementedException();
    }

    internal void RaiseRemoveHealthLevelItemEvent(string name, float trigger, float duration, float increase, bool isPercentage)
    {
        throw new NotImplementedException();
    }
}