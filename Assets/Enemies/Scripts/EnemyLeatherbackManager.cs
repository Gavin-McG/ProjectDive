using UnityEngine;

public class EnemyLeatherbackManager : MonoBehaviour
{
    [Header("Drag Player Here")]
    [SerializeField] Transform playerTransform;

    [SerializeField] float attackRadius;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerTransform == null) GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (EnemyTriggersPhase.IsEnemyInRangeOfPlayer(transform.position, playerTransform.position, attackRadius))
        {
            
        }
    }
}
