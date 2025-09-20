using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class Score : SingletonMonobehaviour<Score>
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI currentScoreUI;

    [Header("Scoring Weights")]
    [SerializeField] private int pointsPerDamageDealt = 10;
    [SerializeField] private int pointsLostPerDamageTaken = 5;
    [SerializeField] private int pointsLostPerSecond = 2;
    [SerializeField] private int levelCompleteBonus = 1000;

    // Totals
    private int damageTaken;
    private int damageDealt;
    private readonly List<float> completedLevelTimes = new(); // only finished levels

    // Live timer for the current level
    private float currentLevelElapsed = 0f;
    private bool timerRunning = false;
    private Coroutine timePenaltyRoutine;

    private int score;

    private void Awake()
    {
        base.Awake();
        ResetAll();
    }

    public void StartLevelTimer()
    {
        currentLevelElapsed = 0f;
        timerRunning = true;

        if (timePenaltyRoutine != null) StopCoroutine(timePenaltyRoutine);
        timePenaltyRoutine = StartCoroutine(TimePenaltyTick());
    }

    public void EndLevelTimer()
    {
        if (!timerRunning) return;

        timerRunning = false;
        if (timePenaltyRoutine != null)
        {
            StopCoroutine(timePenaltyRoutine);
            timePenaltyRoutine = null;
        }

        // Store the finished level time, then clear the live clock
        completedLevelTimes.Add(currentLevelElapsed);
        currentLevelElapsed = 0f;

        RecalculateScore(); // bonus applied via formula below
    }

    public void IncreaseDamageTaken(int damage)
    {
        if (damage <= 0) return;
        damageTaken += damage;
        RecalculateScore();
    }

    public void IncreaseDamageDealt(int damage)
    {
        if (damage <= 0) return;
        damageDealt += damage;
        RecalculateScore();
    }

    private float TotalTimeSeconds =>
        completedLevelTimes.Sum() + (timerRunning ? currentLevelElapsed : 0f);

    private void RecalculateScore()
    {
        int fromDamage = (damageDealt * pointsPerDamageDealt)
                       - (damageTaken * pointsLostPerDamageTaken);

        int timePenalty = Mathf.RoundToInt(TotalTimeSeconds * pointsLostPerSecond);

        int levelsCompleted = completedLevelTimes.Count;
        int computed = fromDamage - timePenalty + (levelsCompleted * levelCompleteBonus);

        score = Mathf.Max(0, computed);
        UpdateScoreUI();
    }

    private IEnumerator TimePenaltyTick()
    {
        var wait = new WaitForSeconds(1f);
        while (timerRunning)
        {
            yield return wait;
            if (!timerRunning) break;

            currentLevelElapsed += 1f; // one second elapsed
            RecalculateScore();
        }
    }

    private void UpdateScoreUI()
    {
        if (currentScoreUI != null)
            currentScoreUI.SetText(score.ToString());
    }

    public int GetScore()
    {
        return score;
    }

    public void ResetAll()
    {
        damageTaken = 0;
        damageDealt = 0;
        completedLevelTimes.Clear();
        currentLevelElapsed = 0f;
        timerRunning = false;

        if (timePenaltyRoutine != null)
        {
            StopCoroutine(timePenaltyRoutine);
            timePenaltyRoutine = null;
        }

        score = 0;
        UpdateScoreUI();
    }
}