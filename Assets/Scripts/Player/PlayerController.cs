using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(CharacterState))]
[RequireComponent(typeof(CharacterCombat))]
[RequireComponent(typeof(CharacterMovement))]
public class PlayerController : MonoBehaviour
{
    private CharacterMovement characterMovement;
    private CharacterState characterState;
    private CharacterCombat characterCombat;

    private Player player;
    public float maxMoveSelectDistance = 30f;

    public TargetEnemy targetEnemy;
    public bool isAiming = false;


    private void Awake()
    {
        // Load components
        player = GetComponent<Player>();
        characterMovement = GetComponent<CharacterMovement>();
        characterState = GetComponent<CharacterState>();
        characterCombat = GetComponent<CharacterCombat>();
    }

    private void Start()
    {
        GameEventManager.Instance.targetEvents.OnAimEnemy += HandleOnAimEnemy;
        GameEventManager.Instance.targetEvents.OnRemoveAim += HandleOnRemoveAim;
        GameEventManager.Instance.targetEvents.OnTargetEnemy += HandleOnTargetEnemy;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.targetEvents.OnAimEnemy -= HandleOnAimEnemy;
        GameEventManager.Instance.targetEvents.OnRemoveAim -= HandleOnRemoveAim;
        GameEventManager.Instance.targetEvents.OnTargetEnemy -= HandleOnTargetEnemy;
    }

    void FixedUpdate()
    {
        if (!player.characterState.isDying && !player.characterState.isDead) {
            Enemy characterTarget = null;
            if (targetEnemy)
            {
                characterTarget = targetEnemy.enemy;
            }
            characterMovement.UpdateSpeed(characterTarget);
        }
    }

    private void HandleOnTargetEnemy(TargetEnemy newTargetEnemy)
    {
        // Check if an enemy is targeted and its not the same target
        if (targetEnemy && targetEnemy != newTargetEnemy)
        {
            targetEnemy.DeselectTarget();
            targetEnemy = null;
        }
        targetEnemy = newTargetEnemy;
    }

    private void HandleOnRemoveAim()
    {
        isAiming = false;
    }

    private void HandleOnAimEnemy()
    {
        isAiming = true;
    }

    private void Update()
    {

        if (!player.characterState.isDying && !player.characterState.isDead && !player.isMovementDisabled){
            MouseInput();
            KeyInput();
            //CastAbility();

            if (targetEnemy != null)
            {
                characterCombat.AttemptAttack(targetEnemy.enemy, false, 0);
            }

            if (characterState.isMoving)
            {
                characterMovement.UpdateCharacterMovement();
            }
        }
    }

    private void MouseInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return; // clicked UI, ignore gameplay

            if (!isAiming) // Right click
            {
                if (targetEnemy)
                {
                    targetEnemy.DeselectTarget();
                    targetEnemy = null;
                }
                MoveToClickPosition();
            }
            // Clicked on an already selected enemy
            else
            {
                player.abilityActivationEvents.CallSetActiveAbilityEvent(player.baseAbility);
/*                if (characterCombat.activeAbility.currentAbility.abilityDetails.isEnemyTargetable && targetEnemy)
                {
                    characterCombat.AttackEnemy(targetEnemy.enemy);
                }*/
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject() || player.activeAbility.GetStagedAbility() == null || player.activeAbility.GetStagedAbility().abilityDetails.isPassive)
                return; // clicked UI, ignore gameplay

            player.abilityActivationEvents.CallActivateStagedAbilityEvent();

/*            if (player.activeAbility.stagedAbility.abilityDetails.isEnemyTargetable && targetEnemy)
            {
                characterCombat.AttackEnemy(targetEnemy.enemy);
            }*/
        }
    }

    private void KeyInput()
    {

        // No enemy targetable ie can be triggered
        if (player.activeAbility.stagedAbility != null && !player.activeAbility.stagedAbility.abilityDetails.isPassive && !player.activeAbility.stagedAbility.abilityDetails.isEnemyTargetable && Input.GetKeyDown(player.activeAbility.stagedAbility.abilityDetails.triggerKey))
        {
            player.abilityActivationEvents.CallActivateStagedAbilityEvent();
            if (!player.activeAbility.currentAbility.abilityDetails.isSelfEffect)
            {
                characterCombat.TriggerAbility();
            } else
            {
                // Use ability for healing spells
                characterCombat.activeAbility.currentAbility.AttemptToUseSuccessful(player);
            }
            player.abilityActivationEvents.CallSetActiveAbilityEvent(player.baseAbility);
        }
    }

    private void MoveToClickPosition()
    {
        Vector3 worldPosition = GetClickPosition();

        int areaMask = NavMesh.AllAreas;
        // Project to navmesh if the click is off-mesh
        if (NavMesh.SamplePosition(worldPosition, out var hit, maxMoveSelectDistance, areaMask))
            worldPosition = hit.position;

        characterMovement.MoveToPosition(worldPosition);
    }

    private Vector3 GetClickPosition()
    {
        Camera mainCamera = null;
        if (!mainCamera) mainCamera = Camera.main;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0f; // Ensure z = 0 for 2D
        return worldPosition;
    }


    public void OnDeath()
    {
        GameManager.Instance.SetStateGameOver();
    }

}