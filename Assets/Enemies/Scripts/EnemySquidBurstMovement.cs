using UnityEngine;

public class EnemySquidBurstMovement : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] private float burstDistance = 1.5f;

    EnemySquidSpriteController squidSR;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        squidSR = transform.GetChild(0).GetComponent<EnemySquidSpriteController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SquidBurstMovement()
    {
        rb.totalForce = Vector2.zero;
        
        float verticalMovement = Random.Range(0.0f, 0.707f);
        int movementDirection = Random.Range(0, 2);

        float horizontalMovement = movementDirection == 0 ? -1 : 1;
        Vector2 directionVector = new Vector2(horizontalMovement, verticalMovement);
        directionVector.Normalize();
        
        rb.AddForce(directionVector * burstDistance);
        
        squidSR.ToggleIsMoving();
        squidSR.DetermineDirection(horizontalMovement);
    }
}
