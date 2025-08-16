using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class AnimateEnemy : MonoBehaviour
{
    private Enemy enemy;
    [SerializeField] private float directionThreshold = 0.05f;

    private void Awake()
    {
        // Load components
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        enemy.movementEvents.OnFaceLeft += HandleFaceLeftAnimation;
        enemy.movementEvents.OnFaceRight += HandleFaceRightAnimation;
        enemy.movementEvents.OnMoveByPosition += HandleMovementAnimation;
        enemy.movementEvents.OnIdle += HandleIdleAnimation;
        enemy.abilityEvents.OnCastAbility += HandleAbilityCastAnimation;
        enemy.abilityEvents.OnMeleeAttack += HandleMeleeAttackAnimation;
    }

    private void OnDisable()
    {
        enemy.movementEvents.OnFaceLeft -= HandleFaceLeftAnimation;
        enemy.movementEvents.OnFaceRight -= HandleFaceRightAnimation;
        enemy.movementEvents.OnMoveByPosition -= HandleMovementAnimation;
        enemy.movementEvents.OnIdle -= HandleIdleAnimation;
        enemy.abilityEvents.OnCastAbility -= HandleAbilityCastAnimation;
        enemy.abilityEvents.OnMeleeAttack -= HandleMeleeAttackAnimation;
    }

    private void ResetAnimationBools()
    {
        enemy.animator.SetBool(Settings.isMoving, false);
        enemy.animator.SetBool(Settings.isIdle, false);
        enemy.animator.SetBool(Settings.isAttacking, false);
    }

    private void HandleFaceLeftAnimation()
    {
        enemy.animator.SetBool(Settings.posTargetRight, false);
        enemy.animator.SetBool(Settings.posTargetLeft, true);
    }

    private void HandleFaceRightAnimation()
    {
        enemy.animator.SetBool(Settings.posTargetRight, true);
        enemy.animator.SetBool(Settings.posTargetLeft, false);
    }

    private void HandleMovementAnimation(float speed)
    {
        ResetAnimationBools();
        enemy.animator.SetBool(Settings.isMoving, true);
    }

    private void HandleIdleAnimation()
    {
        ResetAnimationBools();
        enemy.animator.SetBool(Settings.isMoving, false);
        enemy.animator.SetBool(Settings.isIdle, true);
    }

    private void HandleAbilityCastAnimation()
    {
        ResetAnimationBools();
        enemy.animator.SetBool(Settings.isAttacking, true);
    }

    private void HandleMeleeAttackAnimation()
    {
        ResetAnimationBools();
        enemy.animator.SetBool(Settings.isAttacking, true);
    }

}
