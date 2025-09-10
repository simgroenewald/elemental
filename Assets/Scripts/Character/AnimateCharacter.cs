using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

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
        character.movementEvents.OnDying += HandleDyingAnimation;
        character.abilityEvents.OnCastAbility += HandleAbilityCastAnimation;
        character.abilityEvents.OnMeleeAttack += HandleMeleeAttackAnimation;
    }

    private void OnDisable()
    {
        character.movementEvents.OnFaceLeft -= HandleFaceLeftAnimation;
        character.movementEvents.OnFaceRight -= HandleFaceRightAnimation;
        character.movementEvents.OnMoveByPosition -= HandleMovementAnimation;
        character.movementEvents.OnIdle -= HandleIdleAnimation;
        character.movementEvents.OnDying -= HandleDyingAnimation;
        character.abilityEvents.OnCastAbility -= HandleAbilityCastAnimation;
        character.abilityEvents.OnMeleeAttack -= HandleMeleeAttackAnimation;
    }

    private void ResetAnimationBools()
    {
        character.animator.SetBool(Settings.isMoving, false);
        character.animator.SetBool(Settings.isIdle, false);
        character.animator.SetBool(Settings.isAttacking, false);
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
        character.animator.SetBool(Settings.isMoving, false);
        character.animator.SetBool(Settings.isIdle, true);
    }

    private void HandleAbilityCastAnimation()
    {
        ResetAnimationBools();
        character.animator.SetBool(Settings.isAttacking, true);
    }

    private void HandleMeleeAttackAnimation()
    {
        ResetAnimationBools();
        character.animator.SetBool(Settings.isAttacking, true);
    }

    private void HandleDyingAnimation()
    {
        ResetAnimationBools();
        if (!character.animator.GetBool(Settings.posTargetLeft) && !character.animator.GetBool(Settings.posTargetRight))
            character.animator.SetBool(Settings.posTargetRight, true);
        character.animator.SetBool(Settings.isDying, true);
    }

}
