using System.Collections.Generic;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    public PlayerStatsUI playerStatsUI;
    [SerializeField] private StatIcons statIcons;
    [SerializeField] private Stats stats;

    private void Awake()
    {
        playerStatsUI = GameObject.FindWithTag("PlayerStats").GetComponent<PlayerStatsUI>();
    }

    private void Start()
    {
        SetUpUI();
        UpdateStatsUI();
        stats.OnStatUpdated += UpdateStat;
    }

    private void SetUpUI()
    {
        playerStatsUI.Initialise(stats.GetAllStats());
    }

    private void UpdateStatsUI()
    {
        Dictionary<StatType, float> statsDict = stats.GetAllStats();
        foreach (var stat in statsDict)
        {
            Sprite icon = statIcons.GetIcon(stat.Key);
            playerStatsUI.SetStatUI(stat.Key, icon, stat.Value);
        }
    }

    private void UpdateStat(StatType statType, float value)
    {
        playerStatsUI.UpdateStatUI(statType, value);
    }
}
