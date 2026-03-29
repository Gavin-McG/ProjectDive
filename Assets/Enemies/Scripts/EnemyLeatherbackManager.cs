using UnityEngine;

public class EnemyLeatherbackManager : MonoBehaviour
{
    [Header("Drag Player Here")]
    [SerializeField] Transform playerTransform;

    [SerializeField] float attackRadius = 5.0f;
    [SerializeField] private float chaseRadius = 10.0f;

    EnemyLeatherbackAttack leatherbackAttack;
    EnemyFollowsPlayer leatherbackChase;

    bool idlePhase = true;
    bool attackPhase = false;
    bool attacking = false;
    bool chasePhase = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerTransform == null) GameObject.FindGameObjectWithTag("Player");
        leatherbackAttack = GetComponent<EnemyLeatherbackAttack>();
        leatherbackChase = GetComponent<EnemyFollowsPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (idlePhase && !chasePhase && EnemyTriggersPhase.IsEnemyInRangeOfPlayer(transform.position, playerTransform.position, chaseRadius))
        {
            chasePhase = true;
            idlePhase = false;
            // attackPhase = true;
        }

        else if (chasePhase && !EnemyTriggersPhase.IsEnemyInRangeOfPlayer(transform.position, playerTransform.position, attackRadius))
        {
            leatherbackChase.ToggleChasePlayer(true);
        }

        else if (chasePhase &&
            EnemyTriggersPhase.IsEnemyInRangeOfPlayer(transform.position, playerTransform.position, attackRadius))
        {
            leatherbackChase.ToggleChasePlayer(false);
            attackPhase = true;
            chasePhase = false;
            attacking = false;
        }
        else if (attackPhase && !attacking)
        {
            leatherbackAttack.TriggerLeatherbackAttack();
            attacking = true;
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
}
