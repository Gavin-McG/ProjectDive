using UnityEngine;
using UnityEngine.AI;

public class EnemyAttacksPlayer : MonoBehaviour
{
    [SerializeField] EnemyChaserManager enemyManager;
    [SerializeField] PlayerDashController playerDashController;
    [SerializeField] PlayerSwimController playerSwimController;
    [SerializeField] float attackTimer = 1.0f;
    [SerializeField] float attackDuration = 0.0f;
    
    NavMeshAgent agent;
    private bool releasing = false;
    private bool releasePhase = false;
    [SerializeField] float releaseTimer = 1.0f;
    private float releaseDuration;
    [SerializeField] private float releaseDistance = 4.0f;
    private bool attackPhase = false;

    private Vector3 attackMovement;
    
    EnemyHoundsharkSpriteController sharkSC;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        releaseDuration = releaseTimer;
        sharkSC = transform.GetChild(0).GetComponent<EnemyHoundsharkSpriteController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attackDuration < attackTimer && attackPhase)
        {
            attackDuration += Time.deltaTime;
            // playerSwimController.rb.AddForce(agent.velocity);
            playerSwimController.rb.linearVelocity = attackMovement - attackMovement * (attackDuration/attackTimer);
            agent.velocity = attackMovement - attackMovement * (attackDuration/attackTimer);
            agent.SetDestination(playerSwimController.transform.position);
            if (attackDuration >= attackTimer)
            {
                releasing = true;
                releasePhase = true;
                attackPhase = false;
                playerSwimController.canSwim = true;
                playerDashController.canDash = true;
            }
            sharkSC.ToggleIsMoving(false);
        }
        else if (releasing && releasePhase)
        {
            agent.velocity = -attackMovement;
            // attackDuration = 0;
            // Vector3 dashBackwardsDirectionVector = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f);
            // agent.SetDestination(releaseDistance * dashBackwardsDirectionVector + transform.position);
            // agent.speed = 10.0f;
            // agent.acceleration = 1000.0f;
            releasing = false;
            sharkSC.ToggleIsMoving(true);
        }
        else if (releaseDuration < releaseTimer && releasePhase)
        {
            agent.velocity = -attackMovement - (-attackMovement * (releaseDuration/releaseTimer));
            releaseDuration += Time.deltaTime;
        }
        else if (releaseDuration >= releaseTimer && releasePhase)
        {
            releaseDuration = 0;
            enemyManager.attackPhase = false;
            releasePhase = false;
            playerSwimController.canSwim = true;
            playerDashController.canDash = true;
        }
    }

    public void HoundSharkAttack()
    {
        // Drag player and shark while biting/attacking
        attackDuration = 0;
        Debug.Log(agent.velocity);
        // agent.acceleration = 0;
        playerSwimController.canSwim = false;
        playerDashController.canDash = false;
        attackPhase = true;

        attackMovement = agent.velocity;
    }
}
