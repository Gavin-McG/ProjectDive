using UnityEngine;
using UnityEngine.AI;

public class EnemyLeatherbackAttack : MonoBehaviour
{
    Transform playerTransform;
    Vector2 playerPosition;
    NavMeshAgent agent;
    EnemyLeatherbackManager enemyLeatherbackManager;
    EnemyLeatherbackColliderManager leatherbackCollider;
    PlayerSwimController playerSwimController;
    PlayerDashController playerDashController;
    PlayerHealth playerHealth;
    Rigidbody2D rb;

    [SerializeField] float searchRadius = 50;
    bool attacking = false;
    bool settingup = false;

    [Header("Attack Position")] 
    [SerializeField] private float distanceFromPlayerForAttack = 2.0f;
    Vector2 attackPosition;

    [Header("Chargeing up Attack")] 
    [SerializeField] float chargeDuration = 1.5f;
    float chargeTimer = 0.0f;
    bool charging = false;

    [Header("Crush Duration and Cooldown")]
    [SerializeField] float crushDuration = 3.0f;
    [SerializeField] float crushCooldown = 5.0f;
    float crushTimer = 0.0f;
    
    [SerializeField] AnimationCurve curve;

    [Header("Crush liftoff")] 
    [SerializeField] float crushLiftoffForce = 100.0f;
    [SerializeField] float crushLiftoffTime = 1.5f;
    private bool liftoff = false;
    

    Vector2 nearestWallPoint;
    Vector2 initPosition;
    Vector2 crushDirection;
    Vector2 playerLeatherbackDistance;
    
    bool crushed = false;

    //Debugging
    // public bool TriggerAttack = false;
    
    void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        playerPosition = playerTransform.position;
        
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        if (enemyLeatherbackManager == null)
        {
            enemyLeatherbackManager = GetComponent<EnemyLeatherbackManager>();
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        leatherbackCollider = transform.GetChild(0).GetComponent<EnemyLeatherbackColliderManager>();
        if (!leatherbackCollider) Debug.Log("Leatherback Collider not found");

        playerSwimController = playerTransform.GetComponent<PlayerSwimController>();
        playerDashController = playerTransform.GetComponent<PlayerDashController>();
        playerHealth = playerTransform.GetComponent<PlayerHealth>();

        crushTimer = crushDuration;
        chargeTimer = chargeDuration;

        //TEMPORARY
        // agent.acceleration = crushAcceleration;
        // agent.speed = crushSpeed;
        // initPosition = transform.position;
        // crushTimer = 0.0f;
        // nearestWallPoint = EnemyTriggersPhase.FindNearestWallPoint(playerTransform, searchRadius);
    }

    // Update is called once per frame
    void Update()
    {
        if (!charging && !attacking && settingup)
        {
            if (EnemyTriggersPhase.IsEnemyInRangeOfPlayer(attackPosition, initPosition, 0.1f)) {
                charging = true;
                agent.isStopped = true;
                agent.enabled = false;
                settingup = false;
            }
            else
            {
                initPosition = transform.position;
                playerPosition = playerTransform.position;
                nearestWallPoint = EnemyTriggersPhase.FindNearestWallPoint(playerTransform, searchRadius);
                crushDirection = nearestWallPoint - playerPosition;
                crushDirection.Normalize();
                attackPosition = -crushDirection * distanceFromPlayerForAttack + playerPosition;
                agent.SetDestination(attackPosition);
            }
        }
        if (charging && !attacking && chargeTimer < chargeDuration)
        {
            leatherbackCollider.ToggleAiming(true);
            initPosition = transform.position;
            nearestWallPoint = EnemyTriggersPhase.FindNearestWallPoint(playerTransform, searchRadius);
            crushDirection = nearestWallPoint - initPosition;
            crushDirection.Normalize();
            chargeTimer += Time.deltaTime;
        }
        else if (charging && !attacking && chargeTimer >= chargeDuration)
        {
            charging = false;
            attacking = true;
            rb.position = agent.nextPosition;
            leatherbackCollider.ToggleAiming(false);
            leatherbackCollider.ToggleCrushing(true);
        }
        
        if (!charging && attacking && crushTimer < crushDuration)
        {
            crushTimer += Time.deltaTime;
            float value = curve.Evaluate(crushTimer / crushDuration);
            rb.position = Vector3.Lerp(initPosition, nearestWallPoint, value);
            // transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
        }

        if (crushTimer >= crushDuration && attacking)
        {
            if (crushed)
            {
                Debug.Log("Crushed!");
                PlayerHealthToZero();
            }
            leatherbackCollider.ToggleCrushing(false);
            crushTimer += Time.deltaTime;
            if (!liftoff && crushTimer >= crushLiftoffTime + crushDuration)
            {
                rb.AddForce(-crushDirection * crushLiftoffForce);
                liftoff = true;
            }
        }

        if (crushTimer >= crushDuration + crushCooldown && attacking)
        {
            attacking = false;
            enemyLeatherbackManager.ToggleAttackPhase(false);
            enemyLeatherbackManager.ToggleIdlePhase(true);
            agent.enabled = true;
            agent.isStopped = false;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            leatherbackCollider.ResetRotation();
        }
    }

    public void TriggerLeatherbackAttack()
    {
        initPosition = transform.position;
        crushTimer = 0.0f;
        chargeTimer = 0.0f;
        playerPosition = playerTransform.position;
        nearestWallPoint = EnemyTriggersPhase.FindNearestWallPoint(playerTransform, searchRadius);
        crushDirection = nearestWallPoint - initPosition;
        crushDirection.Normalize();
        liftoff = false;
        attacking = false;
        charging = false;
        settingup = true;
        // charging = true;

        attackPosition = -crushDirection * distanceFromPlayerForAttack + playerPosition;
        agent.isStopped = false;
        agent.SetDestination(attackPosition);
    }

    public void StopLeatherbackAttack()
    {
        attacking = false;
        enemyLeatherbackManager.ToggleAttackPhase(false);
        enemyLeatherbackManager.ToggleIdlePhase(true);
        agent.enabled = true;
        agent.isStopped = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;

        crushTimer = crushDuration;
        chargeTimer = chargeDuration;
        liftoff = true;
        attacking = true;
        charging = true;
        settingup = false;

    }

    public void SetPlayerLeatherbackDistance()
    {
        playerLeatherbackDistance = transform.position - playerTransform.position;
        Debug.Log(playerLeatherbackDistance);
    }

    public void PlayerCrushingMovement()
    {
        SetPlayerLeatherbackDistance();
        Rigidbody2D playerRb = playerTransform.GetComponent<Rigidbody2D>();
        CircleCollider2D[] playerColliders = playerTransform.GetComponents<CircleCollider2D>();
        // foreach (CircleCollider2D playerCollider in playerColliders)
        // {
        //     if (!playerCollider.isTrigger)
        //     {
        //         int playerLayer = LayerMask.NameToLayer("Player");
        //         int enemyLayer = LayerMask.NameToLayer("Lighting-Tiles");
        //         Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        //     }
        // }
        playerSwimController.canSwim = false;
        playerDashController.canDash = false;
        playerRb.position = rb.position + playerLeatherbackDistance;
        // playerTransform.position = new Vector2(transform.position.x, transform.position.y) + playerLeatherbackDistance;
        crushed = true;
    }

    public void PlayerHealthToZero()
    {
        playerHealth.SetHealth(0.0f);
    }

    public bool GetCharging()
    {
        return charging;
    }

    public bool GetCrushing()
    {
        return attacking;
    }

    public float GetCrushingProgress()
    {
        return crushTimer / crushDuration;
    }

    public float GetChargingProgress()
    {
        return chargeTimer / chargeDuration;
    }
}
