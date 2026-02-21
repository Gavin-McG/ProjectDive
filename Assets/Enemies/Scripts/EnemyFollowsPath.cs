using UnityEngine;
using UnityEngine.AI;

public class EnemyFollowsPath : MonoBehaviour
{
    [SerializeField] Transform[] pathPoints;
    [SerializeField] float pathSpeed = 1.0f;
    [SerializeField] float pathAccleration = 1.0f;
    private int pathPointCounter = 0;
    
    NavMeshAgent agent;
    bool followPath = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (followPath)
        {
            if (Vector3.Distance(transform.position, pathPoints[pathPointCounter].position) < 1.0f)
            {
                pathPointCounter = (pathPointCounter + 1) % pathPoints.Length;
            }
            agent.SetDestination(pathPoints[pathPointCounter].position);
        }
    }

    public void ToggleFollowPath(bool willFollowPath)
    {
        followPath = willFollowPath;
        if (followPath)
        {
            agent.speed = pathSpeed;
            agent.acceleration = pathAccleration;
        }
    }
}
