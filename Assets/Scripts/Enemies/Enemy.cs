using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Enemy : Character
{
    public EnemyDetailsSO enemyDetails;
    public StatsSO enemyStatsSO;
    public DungeonRoom room;
    public Image healthBarBackground;
    public Image healthBarFill;
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        Initialise();
    }

    protected virtual void Initialise()
    {
        stats.Initialise(enemyDetails.statsSO[GameManager.Instance.currentLevelIndex]);
        base.Initialise(enemyDetails);
    }

    public void SetRoom(DungeonRoom room)
    {
        this.room = room;
    }
}
