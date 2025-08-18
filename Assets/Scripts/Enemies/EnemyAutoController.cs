using System;
using UnityEngine;
using UnityEngine.AI;

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

    private void Start()
    {
        lineOfSight = enemy.enemyDetails.lineOfSight;
    }

    void FixedUpdate()
    {
        float distanceFromPlayer = Vector2.Distance(player.transform.position, enemy.transform.position);
        Player characterTarget = null;
        if (distanceFromPlayer < enemy.activeAbility.GetCurrentAbility().abilityDetails.range)
        {
            characterTarget = player;
        }
        characterMovement.UpdateSpeed(characterTarget);
    }

    // Update is called once per frame
    void Update()
    {
        characterCombat.AttemptAttack(player, true, lineOfSight);
        if (characterState.isMoving)
        {
            characterMovement.UpdateCharacterMovement();
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lineOfSight);
    }
}
