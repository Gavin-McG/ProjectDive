using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChaserManager : MonoBehaviour
{
    [SerializeField] protected Transform playerTransform;
    [SerializeField] protected float chaseTriggerRadius = 5.0f;
    [SerializeField] protected float chaseRadius = 10.0f;
    protected bool neutralPhase = false;
    protected bool chasePlayerPhase = false;
    protected bool damagePhase = false;
    protected bool idlePhase = true;
    protected bool idling = false;
    public bool attackPhase = false;
    protected bool attacking = false;
    protected NavMeshAgent agent;
    
    protected EnemyFollowsPlayer enemyFollows;
    protected EnemyFollowsPath enemyPathing;
    protected EnemyAttacksPlayer enemyAttacking;

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !attackPhase)
        {
            attackPhase = true;
        }
    }
}
