using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementToItemsListMapperSO", menuName = "Mappers/ElementToItemsListMapperSO")]
public class ElementToItemsListMapperSO : ScriptableObject
{
    [SerializeField] private List<ElementToItemsList> elementToItemsList;
    private Dictionary<ElementTheme, List<GameObject>> elementToItemDictionary = new Dictionary<ElementTheme, List<GameObject>>();

    public void Initialise()
    {
        foreach (var elementToItems in elementToItemsList)
        {
            elementToItemDictionary[elementToItems.theme] = elementToItems.items;
        }
    }

    public Dictionary<ElementTheme, List<GameObject>> GetElementToItemsListDict()
    {
        return elementToItemDictionary;
    }
}

[Serializable]
public class ElementToItemsList
{
    public ElementTheme theme;
    public List<GameObject> items;
}