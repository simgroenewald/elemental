using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetailSO", menuName = "Scriptable Objects/MovementDetailSO")]
public class MovementDetailSO : ScriptableObject
{
    public float minMovementSpeed = 8f;
    public float maxMovementSpeed = 8f;
    public float currentMovementSpeed = 8f;

    public float GetMoveSpeed()
    {
        return currentMovementSpeed;
    }

}
