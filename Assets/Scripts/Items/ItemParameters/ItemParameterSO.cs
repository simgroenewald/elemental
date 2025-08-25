using UnityEngine;

[CreateAssetMenu(fileName = "ItemParameterSO", menuName = "Scriptable Objects/ItemParameterSO")]
public class ItemParameterSO : ScriptableObject
{
    [field: SerializeField]
    public string ParameterName { get; private set; }


}
