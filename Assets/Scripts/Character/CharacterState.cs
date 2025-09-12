using UnityEngine;

public class CharacterState : MonoBehaviour
{
    public bool isMoving;
    public bool isIdle;
    public bool isAttacking;
    public bool isDying;
    public bool isDead;
    public bool isHurt;
    public bool posTargetRight;
    public bool posTargetLeft;

    private void Start()
    {
        SetToIdle();
    }

    private void ResetStates()
    {
        isMoving = false;
        isAttacking = false;
        isIdle = false;
        isDying = false;
        isDead = false;
        isHurt = false;
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

    public void SetToHurt()
    {
        ResetStates();
        isHurt = true;
    }

    public void SetToDying()
    {
        ResetStates();
        isDying = true;
    }

    public void Die()
    {
        ResetStates();
        isDead = true;
    }
}
