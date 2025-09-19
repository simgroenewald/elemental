using System.Collections;
using System.Collections.Generic;
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
    private List<ICastable> projectiles;

    private void Awake()
    {
        activeAbility = GetComponent<ActiveAbility>();
        abilityEvents = GetComponent<AbilityEvents>();
        abilitySetupEvent = GetComponent<AbilitySetupEvent>();
    }

    private void OnEnable()
    {
        abilitySetupEvent.OnSingleMovementAbilitySetup += OnSingleMovementAbilitySetup;
        abilitySetupEvent.OnSingleStaticAbilitySetup += OnSingleStaticAbilitySetup;
        abilitySetupEvent.OnMultiAbilitySetup += OnMultiAbilitySetup;
    }

    private void OnDisable()
    {
        abilitySetupEvent.OnSingleMovementAbilitySetup -= OnSingleMovementAbilitySetup;
        abilitySetupEvent.OnSingleStaticAbilitySetup -= OnSingleStaticAbilitySetup;
        abilitySetupEvent.OnMultiAbilitySetup -= OnMultiAbilitySetup;
    }

    private void Update()
    {
        coolDownTimer -= Time.deltaTime;
    }

    private void OnSingleMovementAbilitySetup(AbilitySetupEvent abilitySetupEvent, OnAbilitySetupEventArgs onAbilitySetupEventArgs)
    {
        if (onAbilitySetupEventArgs.cast)
        {
            SetCastProjectile(
                onAbilitySetupEventArgs.aimAngle, 
                onAbilitySetupEventArgs.abilityAimAngle, 
                onAbilitySetupEventArgs.abilityAimDirectionVector,
                onAbilitySetupEventArgs.direction,
                onAbilitySetupEventArgs.characterCaster,
                onAbilitySetupEventArgs.characterTarget);
        }
    }

    private void OnSingleStaticAbilitySetup(AbilitySetupEvent abilitySetupEvent, OnAbilitySetupEventArgs onAbilitySetupEventArgs)
    {

            SetCastProjectile(onAbilitySetupEventArgs.characterCaster, onAbilitySetupEventArgs.characterTarget);
    }

    private void OnMultiAbilitySetup(AbilitySetupEvent abilitySetupEvent, OnAbilitySetupEventArgs onAbilitySetupEventArgs)
    {
        SetCastProjectiles(onAbilitySetupEventArgs.characterCaster, onAbilitySetupEventArgs.characterTargets);
    }

    private void SetCastProjectile(float aimAngle, float abilityAimAngle, Vector3 abilityAimDirectionVector, TargetDirection direction, Character characterCaster, Character characterTarget)
    {
        ProjectileDetailsSO currentProjectile = activeAbility.GetCurrentProjectile();
        
        if (currentProjectile != null)
        {
            GameObject projectilePrefab = currentProjectile.projectilePrefabArray[Random.Range(0, currentProjectile.projectilePrefabArray.Length)];

            Vector3 spawnPosition = activeAbility.GetCastPosition(direction);
            // Ensure projectile spawns at Z = 0 for 2D consistency
            spawnPosition.z = 0f;
            
            projectile = (ICastable)PoolManager.Instance.ReuseComponent(projectilePrefab, spawnPosition, Quaternion.identity);

            projectile.InitialiseProjectile(currentProjectile, activeAbility.currentAbility, aimAngle, abilityAimAngle, abilityAimDirectionVector, characterCaster, characterTarget);

        }
    }

    private void SetCastProjectile(Character characterCaster, Character characterTarget)
    {
        ProjectileDetailsSO currentProjectile = activeAbility.GetCurrentProjectile();

        if (currentProjectile != null)
        {
            GameObject projectilePrefab = currentProjectile.projectilePrefabArray[Random.Range(0, currentProjectile.projectilePrefabArray.Length)];

            Vector3 spawnPosition = activeAbility.GetCastPosition(characterTarget.GetComponent<Transform>().position);
            spawnPosition.z = 0f;

            projectile = (ICastable)PoolManager.Instance.ReuseComponent(projectilePrefab, spawnPosition, Quaternion.identity);

            projectile.InitialiseProjectile(currentProjectile, activeAbility.GetCurrentAbility(), characterCaster, characterTarget);
        }
    }

    private void SetCastProjectiles(Character characterCaster, List<Character> characterTargets)
    {
        projectiles = new List<ICastable>();
        foreach (Character target in characterTargets)
        {
            ProjectileDetailsSO currentProjectile = activeAbility.GetCurrentProjectile();

            if (currentProjectile != null && !target.characterState.isDead && !target.characterState.isDying)
            {
                GameObject projectilePrefab = currentProjectile.projectilePrefabArray[Random.Range(0, currentProjectile.projectilePrefabArray.Length)];

                Vector3 spawnPosition = activeAbility.GetCastPosition(target.GetComponent<Transform>().position);
                spawnPosition.z = 0f;

                projectile = (ICastable)PoolManager.Instance.ReuseComponent(projectilePrefab, spawnPosition, Quaternion.identity);

                projectile.InitialiseProjectile(currentProjectile, activeAbility.GetCurrentAbility(), characterCaster, target);

                projectiles.Add(projectile);
            }
        }
    }

    public void OnCastAbility()
    {
        if (activeAbility.currentAbility.abilityDetails.isMultiTarget)
        {
            SoundEffectManager.Instance.PlaySoundEffect(activeAbility.currentAbility.abilityDetails.abilityAttackSound);
            foreach (var projectile in projectiles)
            {
                projectile.Cast();
            }      
        } else
        {
            SoundEffectManager.Instance.PlaySoundEffect(activeAbility.currentAbility.abilityDetails.abilityAttackSound);
            projectile.Cast();
        }

    }

    public void OnAbilityCasted()
    {
        abilityEvents.RaiseAbilityCastedEvent();
    }

    /// <summary>
    /// Reset cooldown timer
    /// </summary>
}
