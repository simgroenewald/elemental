using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetEvents", menuName = "Events/TargetEvents")]
public class TargetEvents : ScriptableObject
{
    public event Action<TargetEnemy> OnTargetEnemy;
    public event Action OnAimEnemy;
    public event Action OnRemoveAim;

    public void RaiseOnTargetEnemy(TargetEnemy targetEnemy)
    {
        OnTargetEnemy?.Invoke(targetEnemy);
    }

    public void RaiseOnAimEnemy()
    {
        OnAimEnemy?.Invoke();
    }

    public void RaiseOnRemoveAim()
    {
        OnRemoveAim?.Invoke();
    }
}