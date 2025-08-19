using UnityEngine;

[DisallowMultipleComponent]
public class Projectile : MonoBehaviour, ICastable
{
    [SerializeField] private TrailRenderer trailRenderer;

    private float projectileRange = 0f;
    private float projectileSpeed;
    private Vector3 castDirectionVector;
    private float castDirectionAngle;
    private SpriteRenderer spriteRenderer;
    private ProjectileDetailsSO projectileDetails;
    private float projectileChargeTimer;
    private bool isProjectileMaterialSet = false;
    private bool overrideProjectileMovement;
    private Character characterTarget;

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
                characterTarget.healthEvents.RaiseReduceHealthEvent(projectileDetails.projectileDamage);
                DisableProjectile();
            }
            // Check if the projectile reached the target
        } else
        {
            DisableProjectile();
        }

    }

    public void InitialiseProjectile(ProjectileDetailsSO projectileDetails, float castAngle, float castPointAngle, float projectileSpeed, Vector3 targetDirectionVector, Character characterTarget, bool overrideProjectileMovement = false)
    {
        // Initialise Projectile
        this.projectileDetails = projectileDetails;
        this.characterTarget = characterTarget;

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

        projectileRange = projectileDetails.projectileRange;
        this.projectileSpeed = projectileSpeed;
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
}
