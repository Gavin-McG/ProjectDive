using UnityEngine;
using UnityEngine.AI;

public class EnemyLeatherbackAttack : MonoBehaviour
{
    Transform playerTransform;
    NavMeshAgent agent;

    [SerializeField] float searchRadius = 50;
    bool attacking = false;

    [Header("Crush Attack Acceleration and Speed")]
    [SerializeField] private float crushDuration = 3.0f;
    // [SerializeField] private float crushSpeed = 8.0f;
    float crushTimer = 0.0f;
    [SerializeField] AnimationCurve curve;

    private Vector2 nearestWallPoint;

    Transform initTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        //TEMPORARY
        // agent.acceleration = crushAcceleration;
        // agent.speed = crushSpeed;
        initTransform = transform;
        crushTimer = 0.0f;
        
        nearestWallPoint = EnemyTriggersPhase.FindNearestWallPoint(playerTransform, searchRadius);
    }

    // Update is called once per frame
    void Update()
    {
        // Vector2 nearestWallPoint= EnemyTriggersPhase.FindNearestWallPoint(playerTransform, searchRadius);
        // if (nearestWallPointBool.HasValue)
        // {
        //     Vector2 nearestWallPoint = nearestWallPointBool.Value;
        //     Debug.Log(nearestWallPoint);
        //     agent.SetDestination(nearestWallPoint);
        //     agent.nextPosition = new Vector3(agent.nextPosition.x, agent.nextPosition.y, agent.nextPosition.z);
        // }

        if (crushTimer < crushDuration)
        {
            crushTimer += Time.deltaTime;
            float value = curve.Evaluate(crushTimer / crushDuration);
            transform.position = Vector3.Lerp(initTransform.position, nearestWallPoint, value);
            // transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
        }
    }

    public void TriggerLeatherbackAttack()
    {
        attacking = true;
    }
}
