using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityUnlockedUI : MonoBehaviour
{
    [SerializeField] private Image abilityImage;
    [SerializeField] TMP_Text abilityName;
    [SerializeField] TMP_Text abilityDescription;
    [SerializeField] GameObject abilityDetailsGO;
    [SerializeField] SoundEffectSO clickSound;

    public event Action OnContinue;

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void ResetAbilityDetails()
    {
        gameObject.SetActive(false);
        abilityImage.gameObject.SetActive(false);
        abilityName.text = string.Empty;
        abilityDescription.text = string.Empty;
        abilityDetailsGO.SetActive(false);
    }

    public void SetAbilityDetails(Sprite sprite, string name, string description)
    {
        abilityImage.gameObject.SetActive(true);
        abilityImage.sprite = sprite;
        abilityName.text = name;
        abilityDescription.text = description;
        abilityDetailsGO.SetActive(true);
        gameObject.SetActive(true);
    }
    public void OnButtonClick()
    {
        SoundEffectManager.Instance.PlaySoundEffect(clickSound);
        OnContinue?.Invoke();
    }
}
