using UnityEngine;
using UnityEngine.AI;

public class EnemyHoundSharkManager : EnemyChaserManager
{
    void Start()
    {
        enemyFollows = GetComponent<EnemyFollowsPlayer>();
        // enemyIdle = GetComponent<EnemyIdleInPlace>();
        enemyPathing = GetComponent<EnemyFollowsPath>();
        enemyAttacking = GetComponent<EnemyAttacksPlayer>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (attackPhase && !attacking)
        {
            chasePlayerPhase = false;
            attacking = true;
            enemyAttacking.HoundSharkAttack();
            enemyPathing.ToggleFollowPath(false);  
            enemyFollows.ToggleChasePlayer(false, gameObject);
        }
        else if (!attackPhase && attacking)
        {
            attacking = false;
            idlePhase = true;
            idling = false;
        }
        else if (!chasePlayerPhase && EnemyTriggersPhase.IsEnemyInRangeOfPlayer(transform.position, playerTransform.position, chaseTriggerRadius) && !attackPhase)
        {
            chasePlayerPhase = true;
            idlePhase = false;
            idling = false;
            enemyFollows.ToggleChasePlayer(true, gameObject);
            enemyPathing.ToggleFollowPath(false);
            // enemyIdle.ToggleIdling(false);
        }
        else if (chasePlayerPhase &&
                 !EnemyTriggersPhase.IsEnemyInRangeOfPlayer(transform.position, playerTransform.position, chaseRadius) && !attackPhase)
        {
            chasePlayerPhase = false;
            idlePhase = true;
            enemyFollows.ToggleChasePlayer(false, gameObject);
            // enemyIdle.ToggleIdling(false);
        }
        else if (idlePhase && !idling)
        {
            idling = true;
            enemyPathing.ToggleFollowPath(true);
        }
    }
}
