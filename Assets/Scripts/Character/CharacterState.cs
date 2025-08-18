using UnityEngine;

public class CharacterState : MonoBehaviour
{
    public bool isMoving;
    public bool isIdle;
    public bool isAttacking;
    public bool isDyeing;
    public bool isDead;
    public bool isHurt;
    public bool posTargetRight;
    public bool posTargetLeft;
    private void ResetStates()
    {
        isMoving = false;
        isAttacking = false;
        isIdle = false;
    }

    public void SetToIdle()
    {
        ResetStates();
        isIdle = true;
    }

    public void SetToMoving()
    {
        ResetStates();
        isMoving = true;
    }

    public void SetToAttacking()
    {
        ResetStates();
        isAttacking = true;
    }
}
