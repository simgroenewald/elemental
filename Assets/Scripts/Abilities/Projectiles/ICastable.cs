using UnityEngine;

public interface ICastable
{
    void InitialiseProjectile(
        ProjectileDetailsSO projectileDetails,
        float aimAngle,
        float castPointAngle,
        float projectileSpeed,
        Vector3 targetDirectionVector,
        bool overrideProjectileMovement = false
        );

    GameObject GetGameObject();
}
