using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Enemy))]
public class EnemyAutoController : MonoBehaviour
{
    private Enemy enemy;
    private Player player;
    private NavMeshAgent agent;
    private void Awake()
    {
        // Load components
        enemy = GetComponent<Enemy>();
        player = GameManager.Instance.GetPlayer();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(player.transform.position);
    }
}
