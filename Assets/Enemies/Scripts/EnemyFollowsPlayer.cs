using UnityEngine;
using UnityEngine.AI;

public class EnemyFollowsPlayer : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float enemyChaseSpeed;
    [SerializeField] private float enemyChaseAcceleration;
    
    private NavMeshAgent agent;

    [SerializeField] bool chasePlayer = false;
    bool chasing = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (chasePlayer && chasing)
        {
            agent.SetDestination(player.transform.position);
            
        }
        else if (!chasePlayer && chasing)
        {
            chasing = false;
            agent.velocity = Vector3.zero;
        }
    }

    public void ToggleChasePlayer(bool willChasePlayer)
    {
        chasePlayer = willChasePlayer;
        if (chasePlayer)
        {
            agent.speed = enemyChaseSpeed;
            agent.acceleration = enemyChaseAcceleration;
            chasing = true;
        }
    }
}
