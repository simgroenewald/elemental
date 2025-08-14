using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDetailsSO", menuName = "Scriptable Objects/ProjectileDetailsSO")]
public class ProjectileDetailsSO : ScriptableObject
{
    public string projectileName;
    public bool isPlayerProjectile;
    public bool isAnimated;
    public bool followsTarget;

    public Sprite projectileSprite;
    public GameObject[] projectilePrefabArray;
    public Material projectileMaterial;

    public float chargeTime = 0.1f;
    public Material chargeMaterial;

    public float projectileDamage = 0.1f;
    public float projectileSpeed = 0.1f;
    public float projectileSpeedMin = 20f;
    public float projectileSpeedMax = 20f;
    public float projectileRange = 20f;

    public float projectileRotationSpeed = 20f;

    public bool isProjectileTrail = false;
    public float projectileTrailTime = 3f;

    public Material projectileTrailMaterial;
    [Range(0f, 1f)] public float projectileTrailStartWidth;
    [Range(0f, 1f)] public float projectileTrailEndWidth;
}
