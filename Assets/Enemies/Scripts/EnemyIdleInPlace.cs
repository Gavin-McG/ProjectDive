using UnityEngine;
using UnityEngine.AI;

public class EnemyIdleInPlace : MonoBehaviour
{
    [SerializeField] float idleSpeed = 0.1f;
    [SerializeField] float idleAcceleration = 1.0f;
    [SerializeField] float maxIdleDistance = 1.0f;
    
    bool idling = false;
    NavMeshAgent agent;
    Vector3 idlingPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        idlingPosition = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        if (idling)
        {
            Debug.Log(idlingPosition.x + maxIdleDistance * Mathf.Sin(idleSpeed * 60 * Time.time));
            Vector3 newPosition =
                new Vector3(idlingPosition.x + maxIdleDistance * Mathf.Sin(idleSpeed * 60 * Time.time),
                    idlingPosition.y, idlingPosition.z);
            agent.SetDestination(newPosition);
        }
    }

    public void ToggleIdling(bool willIdle)
    {
        idling = willIdle;
        if (idling)
        {
            agent.speed = idleSpeed;
            agent.acceleration = idleAcceleration;
            idlingPosition = transform.position;
        }
    }
}
