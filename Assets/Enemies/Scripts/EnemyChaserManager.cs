using UnityEditor;
using UnityEngine;

public class EnemyChaserManager : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] float chaseTriggerRadius = 5.0f;
    [SerializeField] float chaseRadius = 10.0f;
    bool neutralPhase = false;
    bool chasePlayerPhase = false;
    bool damagePhase = false;
    bool idlePhase = true;
    bool idling = false;

    EnemyTriggersPhase enemyTriggers;
    EnemyFollowsPlayer enemyFollows;
    EnemyFollowsPath enemyPathing;

    void Start()
    {
        enemyTriggers = GetComponent<EnemyTriggersPhase>();
        enemyFollows = GetComponent<EnemyFollowsPlayer>();
        // enemyIdle = GetComponent<EnemyIdleInPlace>();
        enemyPathing = GetComponent<EnemyFollowsPath>();
    }

    void Update()
    {
        if (!chasePlayerPhase && enemyTriggers.IsEnemyInRangeOfPlayer(transform.position, playerTransform.position, chaseTriggerRadius))
        {
            chasePlayerPhase = true;
            idlePhase = false;
            idling = false;
            enemyFollows.ToggleChasePlayer(true);
            enemyPathing.ToggleFollowPath(false);
            // enemyIdle.ToggleIdling(false);
        }
        else if (chasePlayerPhase &&
                 !enemyTriggers.IsEnemyInRangeOfPlayer(transform.position, playerTransform.position, chaseRadius))
        {
            chasePlayerPhase = false;
            idlePhase = true;
            enemyFollows.ToggleChasePlayer(false);
            // enemyIdle.ToggleIdling(false);
        }
        else if (idlePhase && !idling)
        {
            idling = true;
            enemyPathing.ToggleFollowPath(true);
        }
    }
}
