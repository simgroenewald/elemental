using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ActiveAbility))]
[RequireComponent(typeof(AbilityEvents))]
[RequireComponent(typeof(AbilitySetupEvent))]

[DisallowMultipleComponent]
public class CastAbility : MonoBehaviour
{
    private float coolDownTimer = 0f;
    private ActiveAbility activeAbility;
    private AbilityEvents abilityEvents;
    private AbilitySetupEvent abilitySetupEvent;
    private ICastable projectile;

    private void Awake()
    {
        activeAbility = GetComponent<ActiveAbility>();
        abilityEvents = GetComponent<AbilityEvents>();
        abilitySetupEvent = GetComponent<AbilitySetupEvent>();
    }

    private void OnEnable()
    {
        abilitySetupEvent.OnAbilitySetup += OnAbilitySetup;
    }

    private void OnDisable()
    {
        abilitySetupEvent.OnAbilitySetup -= OnAbilitySetup;
    }

    private void Update()
    {
        coolDownTimer -= Time.deltaTime;
    }

    private void OnAbilitySetup(AbilitySetupEvent abilitySetupEvent, OnAbilitySetupEventArgs onAbilitySetupEventArgs)
    {
        if (onAbilitySetupEventArgs.cast)
        {
            if (IsAbilityReadyToCast())
            {
                SetCastProjectile(
                    onAbilitySetupEventArgs.aimAngle, 
                    onAbilitySetupEventArgs.abilityAimAngle, 
                    onAbilitySetupEventArgs.abilityAimDirectionVector,
                    onAbilitySetupEventArgs.direction,
                    onAbilitySetupEventArgs.characterTarget);

                ResetCoolDownTimer();
            }
        }
    }

    private bool IsAbilityReadyToCast()
    {
        if (activeAbility.GetCurrentAbility().isCoolingDown)
            return false;

        if (coolDownTimer > 0f)
            return false;

        return true;

    }

    private void SetCastProjectile(float aimAngle, float abilityAimAngle, Vector3 abilityAimDirectionVector, TargetDirection direction, Character characterTarget)
    {
        ProjectileDetailsSO currentProjectile = activeAbility.GetCurrentProjectile();
        

        if (currentProjectile != null)
        {
            GameObject projectilePrefab = currentProjectile.projectilePrefabArray[Random.Range(0, currentProjectile.projectilePrefabArray.Length)];

            float projectileSpeed = Random.Range(currentProjectile.projectileSpeedMin, currentProjectile.projectileSpeedMax);

            Vector3 spawnPosition = activeAbility.GetCastPosition(direction);
            // Ensure projectile spawns at Z = 0 for 2D consistency
            spawnPosition.z = 0f;
            
            projectile = (ICastable)PoolManager.Instance.ReuseComponent(projectilePrefab, spawnPosition, Quaternion.identity);

            projectile.InitialiseProjectile(currentProjectile, aimAngle, abilityAimAngle, projectileSpeed, abilityAimDirectionVector, characterTarget);

        }
    }

    public void OnCastAbility(GameObject target)
    {
        projectile.Cast();
    }

    public void OnAbilityCasted()
    {
        abilityEvents.RaiseAbilityCastedEvent();
    }

    /// <summary>
    /// Reset cooldown timer
    /// </summary>
    private void ResetCoolDownTimer()
    {
        // Reset cooldown timer
        coolDownTimer = activeAbility.GetCurrentAbility().abilityCooldownTime;
    }
}
