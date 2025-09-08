using UnityEngine;

public interface ICastable
{
    void InitialiseProjectile(
        ProjectileDetailsSO projectileDetails,
        Ability ability,
        float aimAngle,
        float castPointAngle,
        Vector3 targetDirectionVector,
        Character characterTarget,
        bool overrideProjectileMovement = false
        );

    void Cast();

    GameObject GetGameObject();
}
