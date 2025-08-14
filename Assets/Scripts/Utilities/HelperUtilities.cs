using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class HelperUtilities
{
    public static Camera mainCamera;

    public static Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        Vector3 mouseScreenPosition = Input.mousePosition;

        // Clamp mouse position to screen size
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        worldPosition.z = 0f;

        return worldPosition;

    }

    public static float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x);

        float degrees = radians * Mathf.Rad2Deg;

        return degrees;

    }

    public static Vector3 GetDirectionVectorFromAngle(float angle)
    {
        Vector3 directionVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
        return directionVector;
    }

    public static TargetDirection GetTargetDirection(float angleDegrees)
    {
        TargetDirection direction = TargetDirection.Right;

        // Set player direction
        //Right
        if ((angleDegrees >= 0f && angleDegrees <= 90f) || (angleDegrees >= -90f && angleDegrees <= 0f))
        {
            direction = TargetDirection.Right;
        }
        //Left
        if ((angleDegrees > 90f && angleDegrees <= 180f) || (angleDegrees >= -180f && angleDegrees < -90f))
        {
            direction = TargetDirection.Left;
        }

        return direction;

    }

}
