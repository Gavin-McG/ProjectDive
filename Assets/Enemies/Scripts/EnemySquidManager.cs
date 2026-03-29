using UnityEngine;

public class EnemySquidManager : MonoBehaviour
{
    [Header("Squid Trigger Radius to Attack")]
    [SerializeField] float squidTirggerAttackRadius = 2.5f;

    [Header("Drag Player Here")] 
    [SerializeField] Transform playerTransform;

    [Header("Squid Attack Cooldown")] 
    [SerializeField] private float squidAttackCooldown = 4.0f;
    private float squidAttackTimer = 0.0f;

    [Header("Squid Movement")] 
    [SerializeField] private float squidMovementCooldown = 1.0f;
    float squidMovementTimer = 0.0f;

    EnemySquidAttack enemySquidAttack;
    EnemySquidBurstMovement enemySquidMovement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemySquidAttack = GetComponent<EnemySquidAttack>();
        enemySquidMovement = GetComponent<EnemySquidBurstMovement>();
        squidAttackTimer = squidAttackCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (EnemyTriggersPhase.IsEnemyInRangeOfPlayer(transform.position, playerTransform.position,
                squidTirggerAttackRadius) && squidAttackTimer >= squidAttackCooldown)
        {
            squidAttackTimer = 0.0f;
            enemySquidAttack.SpawnInk();
        }
        else
        {
            squidAttackTimer += Time.deltaTime;
        }

        if (squidMovementTimer < squidMovementCooldown)
        {
            squidMovementTimer += Time.deltaTime;
        }
        else if (squidMovementTimer >= squidMovementCooldown)
        {
            squidMovementTimer = 0;
            enemySquidMovement.SquidBurstMovement();
        }
    }
}
