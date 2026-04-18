using UnityEngine;

public class EnemyLeatherbackManager : MonoBehaviour
{
    [Header("Drag Player Here")]
    [SerializeField] Transform playerTransform;

    [SerializeField] float attackRadius = 5.0f;
    [SerializeField] private float chaseRadius = 10.0f;

    EnemyLeatherbackAttack leatherbackAttack;
    EnemyFollowsPlayer leatherbackChase;
    EnemyFollowsPath leatherbackIdle;

    bool idlePhase = true;
    bool attackPhase = false;
    bool attacking = false;
    bool chasePhase = false;
    [SerializeField] bool playerInsideCave = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerTransform == null) playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        leatherbackAttack = GetComponent<EnemyLeatherbackAttack>();
        leatherbackChase = GetComponent<EnemyFollowsPlayer>();
        leatherbackIdle = GetComponent<EnemyFollowsPath>();
        leatherbackIdle.ToggleFollowPath(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (idlePhase & !chasePhase &&
            !EnemyTriggersPhase.IsEnemyInRangeOfPlayer(transform.position, playerTransform.position, chaseRadius))
        {
            // Idle -> Idle
            leatherbackIdle.ToggleFollowPath(true);
        }
        if (idlePhase && !chasePhase && EnemyTriggersPhase.IsEnemyInRangeOfPlayer(transform.position, playerTransform.position, chaseRadius) && playerInsideCave)
        {
            // Idle -> Chase
            leatherbackIdle.ToggleFollowPath(false);
            chasePhase = true;
            idlePhase = false;
            leatherbackChase.ToggleChasePlayer(true, gameObject);
            // attackPhase = true;
        }

        else if (chasePhase &&
                 (!EnemyTriggersPhase.IsEnemyInRangeOfPlayer(transform.position, playerTransform.position, chaseRadius) || !playerInsideCave))
        {
            // Chase -> Idle
            leatherbackChase.ToggleChasePlayer(false, gameObject);
            chasePhase = false;
            idlePhase = true;
        }

        else if (chasePhase &&
            EnemyTriggersPhase.IsEnemyInRangeOfPlayer(transform.position, playerTransform.position, attackRadius))
        {
            // Chase -> Attack
            leatherbackChase.ToggleChasePlayer(false, gameObject);
            attackPhase = true;
            chasePhase = false;
            attacking = false;
        }
        else if (attackPhase && !attacking)
        {
            // Toggle Attacking
            leatherbackAttack.TriggerLeatherbackAttack();
            attacking = true;
        }
        else if (attacking && !playerInsideCave)
        {
            // Attack -> Idle
            leatherbackAttack.StopLeatherbackAttack();
            attacking = false;
            attackPhase = false;
            chasePhase = false;
            idlePhase = true;
        }
    }

    public void ToggleAttackPhase(bool toggle)
    {
        attackPhase = toggle;
        attacking = toggle;
    }

    public void ToggleIdlePhase(bool toggle)
    {
        idlePhase = toggle;
    }
    
    public void TogglePlayerInsideCave(bool toggle)
    {
        playerInsideCave = toggle;
    }

    public bool GetIdle()
    {
        return idlePhase;
    }
    public bool GetChasing()
    {
        return chasePhase;
    }

    public bool GetAttacking()
    {
        return attackPhase;
    }
    
}
