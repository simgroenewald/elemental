using System;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class ItemActionUI : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonPrefab;

    public void AddButton(string name, Action onClickAction)
    {
        GameObject button = Instantiate(buttonPrefab, transform);
        button.GetComponent<Button>().onClick.AddListener(() => onClickAction());
        button.GetComponentInChildren<TMPro.TMP_Text>().text = name;
    }

    internal void Toggle(bool val)
    {
        if (val == true)
            RemoveOldButtons();
        gameObject.SetActive(val);
    }

    public void RemoveOldButtons()
    {
        foreach (Transform transformChildObjects in transform)
        {
            Destroy(transformChildObjects.gameObject);
        }
    }
}
