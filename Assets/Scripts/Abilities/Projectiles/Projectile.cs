using UnityEngine;
using UnityEngine.TextCore.Text;

[DisallowMultipleComponent]
public class Projectile : MonoBehaviour, ICastable
{
    [SerializeField] private TrailRenderer trailRenderer;

    private Character characterCaster;
    private Vector3 castDirectionVector;
    private float castDirectionAngle;
    private SpriteRenderer spriteRenderer;
    private ProjectileDetailsSO projectileDetails;
    private float projectileChargeTimer;
    private bool isProjectileMaterialSet = false;
    private bool overrideProjectileMovement;
    private Character characterTarget;
    private Ability ability;
    private float projectileDamage = 0f;
    private float projectileSpeed = 0f;
    private float projectileRange = 0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (projectileChargeTimer > 0f)
        {
            projectileChargeTimer -= Time.deltaTime;
            return;
        }
        else if (!isProjectileMaterialSet)
        {
            SetProjectileMaterial(projectileDetails.projectileMaterial);
            isProjectileMaterialSet= true;
        }

        // Follow target
        if (characterTarget != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, characterTarget.target.position, projectileSpeed * Time.deltaTime);
        } else
        // Cast for set distance
        {
            // Calculate distance vector to move projectile
            Vector3 distanceVector = castDirectionVector * projectileSpeed * Time.deltaTime;

            transform.position += distanceVector;

            projectileRange -= distanceVector.magnitude;

            if (projectileRange < 0f)
            {
                DisableProjectile();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (characterTarget != null)
        {
            if (collision.gameObject == characterTarget.GetGameObject())
            {
                DealDamage();
                DisableProjectile();
            }
            // Check if the projectile reached the target
        } else
        {
            DisableProjectile();
        }
    }

    public void ManualDisable()
    {
        DisableProjectile();
    }

    public void DealDamage()
    {
        float damage;
        if (ability.abilityDetails.isHurt)
        {
            characterTarget.characterState.SetToHurt();
            characterTarget.movementEvents.RaiseHurt();
        }
        if (ability.abilityDetails.isCritical)
        {
            damage = ability.EvaluateDamageDealingStats(characterCaster, ability.abilityDetails._damage);
        }
        else
        {
            damage = ability.abilityDetails._damage;
        }
        damage = characterTarget.stats.EvaluateDamageTakingStats(damage);
        characterTarget.healthEvents.RaiseReduceHealthEvent(damage);
        if (characterCaster.stats.GetStat(StatType.HealthSteal) > 0)
        {
            float health = characterCaster.stats.GetStat(StatType.HealthSteal) * damage / 100;
            characterCaster.healthEvents.RaiseIncreaseHealthEvent(health);
        }
    }

    public void InitialiseProjectile(ProjectileDetailsSO projectileDetails, Ability ability, float castAngle, float castPointAngle, Vector3 targetDirectionVector, Character characterCaster, Character characterTarget, bool overrideProjectileMovement = false)
    {
        // Initialise Projectile
        this.projectileDetails = projectileDetails;
        this.characterCaster = characterCaster;
        this.characterTarget = characterTarget;
        this.ability = ability;

        this.projectileDamage = ability.abilityDetails._damage;
        this.projectileRange = ability.abilityDetails._range;
        this.projectileSpeed = ability.abilityDetails._projectileSpeed;

        // Set cast direction
        SetCastDirection(projectileDetails, castAngle, castPointAngle, targetDirectionVector);

        spriteRenderer.sprite = projectileDetails.projectileSprite;

        if (projectileDetails.chargeTime > 0f)
        {
            projectileChargeTimer = projectileDetails.chargeTime;
            SetProjectileMaterial(projectileDetails.chargeMaterial);
            isProjectileMaterialSet = false;
        } else
        {
            projectileChargeTimer = 0f;
            //SetProjectileMaterial(projectileDetails.projectileMaterial);
            isProjectileMaterialSet = true;
        }

        this.overrideProjectileMovement = overrideProjectileMovement;

        // InitialiseTrail
        if (projectileDetails.isProjectileTrail)
        {
            trailRenderer.gameObject.SetActive(true);
            trailRenderer.emitting = true;
            trailRenderer.material = projectileDetails.projectileTrailMaterial;
            trailRenderer.startWidth = projectileDetails.projectileTrailStartWidth;
            trailRenderer.endWidth = projectileDetails.projectileTrailEndWidth;
            trailRenderer.time = projectileDetails.projectileTrailTime;
        }
        else
        {
            trailRenderer.emitting = false;
            trailRenderer.gameObject.SetActive(false);
        }
    }

    public void Cast()
    {
        Animator animator = gameObject.GetComponent<Animator>();
        if (animator)
        {
            animator.SetBool(Settings.isTriggered, true);
        }
        gameObject.SetActive(true);
    }

    private void SetCastDirection(ProjectileDetailsSO projectileDetails, float castAngle, float castPointAngle, Vector3 targetDirectionVector)
    {
        if (targetDirectionVector.magnitude < Settings.useTargetAngleDistance)
        {
            castDirectionAngle = castAngle;
        } 
        else
        {
            castDirectionAngle = castPointAngle;
        }

        transform.eulerAngles = new Vector3(0f, 0f, castDirectionAngle);

        castDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(castDirectionAngle);
    }

    private void DisableProjectile()
    {
        gameObject.SetActive(false);
    }

    public void SetProjectileMaterial(Material material)
    {
        spriteRenderer.material = material;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    // Static animated ability
    public void InitialiseProjectile(ProjectileDetailsSO projectileDetails, Ability ability, Character characterCaster, Character characterTarget)
    {
        // Initialise Projectile
        this.projectileDetails = projectileDetails;
        this.characterCaster = characterCaster;
        this.characterTarget = characterTarget;
        this.ability = ability;

        this.projectileDamage = ability.abilityDetails._damage;
        this.projectileRange = ability.abilityDetails._range;
        this.projectileSpeed = ability.abilityDetails._projectileSpeed;

        spriteRenderer.sprite = projectileDetails.projectileSprite;

        projectileChargeTimer = 0f;
        //SetProjectileMaterial(projectileDetails.projectileMaterial);
        isProjectileMaterialSet = true;

        this.overrideProjectileMovement = false;

        trailRenderer.emitting = false;
        trailRenderer.gameObject.SetActive(false);
    }
}
