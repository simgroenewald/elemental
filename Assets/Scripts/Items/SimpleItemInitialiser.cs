using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class SimpleItemInitialiser : MonoBehaviour
{
    [Header("Item Initialisation")]
    [SerializeField] private List<GameObject> items;

    public List<GameObject> GetItemPrefabs()
    {

        return items;
    }
}