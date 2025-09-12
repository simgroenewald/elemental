using UnityEngine;

[RequireComponent(typeof(Character))]
[DisallowMultipleComponent]
public class AnimateCharacter : MonoBehaviour
{
    private Character character;
    [SerializeField] private float directionThreshold = 0.05f;

    private void Awake()
    {
        // Load components
        character = GetComponent<Character>();
    }

    private void Start()
    {
        character.movementEvents.OnFaceLeft += HandleFaceLeftAnimation;
        character.movementEvents.OnFaceRight += HandleFaceRightAnimation;
        character.movementEvents.OnMoveByPosition += HandleMovementAnimation;
        character.movementEvents.OnIdle += HandleIdleAnimation;
        character.movementEvents.OnHurt += HandleHurtAnimation;
        character.movementEvents.OnDying += HandleDyingAnimation;
        character.abilityEvents.OnCastAbility += HandleAttackAnimation;
        character.abilityEvents.OnMeleeAttack += HandleAttackAnimation;
        HandleFaceLeftAnimation();
    }

    private void OnDisable()
    {
        character.movementEvents.OnFaceLeft -= HandleFaceLeftAnimation;
        character.movementEvents.OnFaceRight -= HandleFaceRightAnimation;
        character.movementEvents.OnMoveByPosition -= HandleMovementAnimation;
        character.movementEvents.OnIdle -= HandleIdleAnimation;
        character.movementEvents.OnDying -= HandleDyingAnimation;
        character.abilityEvents.OnCastAbility -= HandleAttackAnimation;
        character.abilityEvents.OnMeleeAttack -= HandleAttackAnimation;
    }

    private void ResetAnimationBools()
    {
        character.animator.SetBool(Settings.isMoving, false);
        character.animator.SetBool(Settings.isIdle, false);
        character.animator.SetBool(Settings.isAttacking, false);
        character.animator.SetBool(Settings.isAttacking2, false);
        character.animator.SetBool(Settings.isHurting, false);
    }

    private void HandleFaceLeftAnimation()
    {
        character.animator.SetBool(Settings.posTargetRight, false);
        character.animator.SetBool(Settings.posTargetLeft, true);
    }

    private void HandleFaceRightAnimation()
    {
        character.animator.SetBool(Settings.posTargetRight, true);
        character.animator.SetBool(Settings.posTargetLeft, false);
    }

    private void HandleMovementAnimation(float speed)
    {
        ResetAnimationBools();
        character.animator.SetBool(Settings.isMoving, true);
    }

    private void HandleIdleAnimation()
    {
        ResetAnimationBools();
        character.animator.SetBool(Settings.isIdle, true);
    }

    private void HandleHurtAnimation()
    {
        ResetAnimationBools();
        character.animator.SetBool(Settings.isHurting, true);
    }

    private void HandleAttackAnimation()
    {
        ResetAnimationBools();
        if (character.activeAbility.currentAbility.abilityDetails.attackAnimationTrigger == null)
        {
            character.animator.SetBool(Settings.isAttacking, true);
        } 
        else
        {
            int attackParamHash = Animator.StringToHash(character.activeAbility.currentAbility.abilityDetails.attackAnimationTrigger);
            character.animator.SetBool(attackParamHash, true);
        }

    }

    private void HandleDyingAnimation()
    {
        ResetAnimationBools();
        if (!character.animator.GetBool(Settings.posTargetLeft) && !character.animator.GetBool(Settings.posTargetRight))
            character.animator.SetBool(Settings.posTargetRight, true);
        character.animator.SetBool(Settings.isDying, true);
    }

}
