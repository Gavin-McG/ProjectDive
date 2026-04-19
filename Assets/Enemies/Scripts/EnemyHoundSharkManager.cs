using UnityEngine;
using UnityEngine.AI;

public class EnemyHoundSharkManager : EnemyChaserManager
{
    EnemyHoundsharkSpriteController sharkSC;
    float prevXPos;
    void Start()
    {
        enemyFollows = GetComponent<EnemyFollowsPlayer>();
        // enemyIdle = GetComponent<EnemyIdleInPlace>();
        enemyPathing = GetComponent<EnemyFollowsPath>();
        enemyAttacking = GetComponent<EnemyAttacksPlayer>();
        agent = GetComponent<NavMeshAgent>();
        sharkSC = transform.GetChild(0).GetComponent<EnemyHoundsharkSpriteController>();
        prevXPos = transform.position.x;
    }

    void Update()
    {
        if (attackPhase && !attacking)
        {
            // Attack
            chasePlayerPhase = false;
            attacking = true;
            enemyAttacking.HoundSharkAttack();
            enemyPathing.ToggleFollowPath(false);  
            enemyFollows.ToggleChasePlayer(false, gameObject);
        }
        else if (!attackPhase && attacking)
        {
            // Attack -> Idle
            attacking = false;
            idlePhase = true;
            idling = false;
        }
        else if (!chasePlayerPhase && EnemyTriggersPhase.IsEnemyInRangeOfPlayer(transform.position, playerTransform.position, chaseTriggerRadius) && !attackPhase)
        {
            // Idle -> Chase
            chasePlayerPhase = true;
            idlePhase = false;
            idling = false;
            enemyFollows.ToggleChasePlayer(true, gameObject);
            enemyPathing.ToggleFollowPath(false);
            sharkSC.ToggleIsMoving(true);
            // enemyIdle.ToggleIdling(false);
        }
        else if (chasePlayerPhase &&
                 !EnemyTriggersPhase.IsEnemyInRangeOfPlayer(transform.position, playerTransform.position, chaseRadius) && !attackPhase)
        {
            // Chase -> Idle
            chasePlayerPhase = false;
            idlePhase = true;
            enemyFollows.ToggleChasePlayer(false, gameObject);
            sharkSC.ToggleIsMoving(false);
            // enemyIdle.ToggleIdling(false);
        }
        else if (idlePhase && !idling)
        {
            // Idle
            idling = true;
            enemyPathing.ToggleFollowPath(true);
        }

        if (chasePlayerPhase)
        {
            // string direction = prevXPos > transform.position.x ? "Left" : "Right";
            // sharkSC.SetCurrentDirection(direction);
            sharkSC.ToggleIsChasing(true);
            sharkSC.ToggleIsIdle(false);
            sharkSC.ToggleIsAttacking(false);
        }
        else if (idlePhase)
        {
            string direction = prevXPos > transform.position.x ? "Left" : "Right";
            sharkSC.SetCurrentDirection(direction);
            sharkSC.ToggleIsChasing(false);
            sharkSC.ToggleIsIdle(true);
            sharkSC.ToggleIsAttacking(false);
        }
        else
        {
            sharkSC.ToggleIsChasing(false);
            sharkSC.ToggleIsIdle(false);
            sharkSC.ToggleIsAttacking(true);
        }

        prevXPos = transform.position.x;
    }
}
