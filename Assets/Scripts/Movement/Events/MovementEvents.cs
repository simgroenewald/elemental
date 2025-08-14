using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementEvents", menuName = "Events/MovementEvents")]
public class MovementEvents : ScriptableObject
{
    public event Action OnFaceLeft;
    public event Action OnFaceRight;
    public event Action<float> OnMoveByPosition;
    public event Action OnIdle;
    public event Action OnAttack;

    public void RaiseFaceLeft()
    {
        OnFaceLeft?.Invoke();
    }

    public void RaiseFaceRight()
    {
        OnFaceRight?.Invoke();
    }

    public void RaiseMoveByPosition(float speed)
    {
        OnMoveByPosition?.Invoke(speed);
    }

    public void RaiseAttack()
    {
        OnAttack?.Invoke();
    }

    public void RaiseIdle()
    {
        OnIdle?.Invoke();
    }
}