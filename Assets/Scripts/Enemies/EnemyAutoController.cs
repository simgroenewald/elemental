using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(CharacterState))]
[RequireComponent(typeof(CharacterCombat))]
public class EnemyAutoController : MonoBehaviour
{
    private Enemy enemy;
    private Player player;
    private CharacterMovement characterMovement;
    private CharacterState characterState;
    private CharacterCombat characterCombat;

    private float lineOfSight;


    private void Awake()
    {
        // Load components
        enemy = GetComponent<Enemy>();
        player = GameManager.Instance.GetPlayer();

        characterMovement = GetComponent<CharacterMovement>();
        characterState = GetComponent<CharacterState>();
        characterCombat = GetComponent<CharacterCombat>();

    }

    private void OnEnable()
    {
        enemy.healthEvents.OnReduceHealth += OnReduceHealth;
    }

    private void OnDisable()
    {
        enemy.healthEvents.OnReduceHealth -= OnReduceHealth;
    }

    private void Start()
    {
        lineOfSight = enemy.characterDetails.stats.GetStat(StatType.LineOfSight);
    }

    void FixedUpdate()
    {
        if (!enemy.characterState.isDying && !enemy.characterState.isDead && !player.characterState.isDead && enemy.room.isEntered)
        {
            float distanceFromPlayer = Vector2.Distance(player.transform.position, enemy.transform.position);
            Player characterTarget = null;
            if (distanceFromPlayer < enemy.activeAbility.GetCurrentAbility().abilityDetails.range)
            {
                characterTarget = player;
            } 
            characterMovement.UpdateSpeed(characterTarget);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemy.characterState.isDying && !enemy.characterState.isDead && !player.characterState.isDead && enemy.room.isEntered) {
            characterCombat.AttemptAttack(player, true, lineOfSight);
            if (characterState.isMoving)
            {
                characterMovement.UpdateCharacterMovement();
            }
            
            float distanceFromPlayer = Vector2.Distance(player.transform.position, enemy.transform.position);
            if (distanceFromPlayer < lineOfSight)
            {
                if (characterCombat.attackTriggered)
                {
                    characterCombat.normalAdvance = true;
                    characterCombat.attackTriggered = false;
                }
            }
        }
        if (enemy.characterState.isDead)
        {
            OnDeath();
        }
    }

    private void OnReduceHealth(float health)
    {
        // If the enemy is idle and has taken damage
        if (enemy.characterState.isIdle)
        {
            TriggerAutoAttack();
        }
    }

    private void TriggerAutoAttack()
    {
        float distanceFromPlayer = Vector2.Distance(player.transform.position, enemy.transform.position);
        if (distanceFromPlayer > lineOfSight && !characterCombat.attackTriggered)
        {
            characterCombat.normalAdvance = false;
            characterCombat.attackTriggered = true;
            characterMovement.MoveToPosition(player.transform.position);
        }
    }

    private void OnDeath()
    {
        GameEventManager.Instance.itemEvents.RaiseDropItemEvent(enemy.room, (int)enemy.characterDetails.stats.GetStat(StatType.Health), enemy.transform);
        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lineOfSight);
    }
}
