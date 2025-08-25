using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField]
    private ActiveBuffItem activeBuffItem;

    [SerializeField]
    private BackpackSO backpack;

    [SerializeField]
    private List<ItemParameter> parametersToModify;

    [SerializeField]
    private List<ItemParameter> itemParameters;

    public void SetItem(ActiveBuffItem activeBuffItemSO, List<ItemParameter> itemParameters)
    {
        if (this.activeBuffItem != null)
        {
            backpack.AddItem(activeBuffItemSO, 1, this.itemParameters);
        }

        this.activeBuffItem = activeBuffItemSO;
        this.itemParameters = new List<ItemParameter>(itemParameters);
        ModifyParameters();
    }

    private void ModifyParameters()
    {
        foreach (var parameter in parametersToModify)
        {
            if(itemParameters.Contains(parameter))
            {
                int index = itemParameters.IndexOf(parameter);
                float updatedValue = itemParameters[index].value + parameter.value;
                itemParameters[index] = new ItemParameter
                {
                    itemParameterSO = parameter.itemParameterSO,
                    value = updatedValue
                };
            }

        }
    }
}
