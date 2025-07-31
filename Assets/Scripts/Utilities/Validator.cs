using UnityEngine;

public static class Validator
{
    public static bool ValidateNullValue(Object _object, string fieldname, UnityEngine.Object objectToCheck)
    {
        if (objectToCheck == null)
        {
            Debug.Log(fieldname + " is null and must contain a value in the object " + _object.name.ToString());
            return true;
        }
        return false;
    }

    public static bool ValidatePositiveValue(Object _object, string fieldname, int valueToCheck, bool zeroAllowed)
    {
        bool error = false;

        if (zeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fieldname + " must contain a positive value or zero in the object " + _object.name.ToString());
                error = true;
            }
        } else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fieldname + " must contain a positive value in the object " + _object.name.ToString());
                error = true;
            }
        }

        return error;

    }
}
