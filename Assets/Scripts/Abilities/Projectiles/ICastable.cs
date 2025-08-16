using UnityEngine;

public interface ICastable
{
    void InitialiseProjectile(
        ProjectileDetailsSO projectileDetails,
        float aimAngle,
        float castPointAngle,
        float projectileSpeed,
        Vector3 targetDirectionVector,
        ITargetable target,
        bool overrideProjectileMovement = false
        );

    void Cast();

    GameObject GetGameObject();
}
