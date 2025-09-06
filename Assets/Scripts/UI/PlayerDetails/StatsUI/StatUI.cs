using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StatUI : MonoBehaviour
{

    [SerializeField] Image statIcon;
    [SerializeField] TMP_Text statName;
    [SerializeField] TMP_Text statValue;

    public void SetStat(Sprite sprite, String statname, float value)
    {
        statIcon.sprite = sprite;
        statName.text = statname;
        statValue.text = value.ToString();
    }

    public void UpdateStat(float value)
    {
        statValue.text = value.ToString();
    }
}
