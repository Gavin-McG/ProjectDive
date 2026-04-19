using UnityEngine;
using UnityEngine.U2D.Animation;

public class EnemyHoundsharkSpriteController : MonoBehaviour
{
    [SerializeField]  private float swimAnimSpeedMoving = 0.15f;
    [SerializeField] private float swimAnimSpeedIdle = 0.4f;

    bool isMoving = false;
    bool isChasing = false;
    private bool isIdle = false;
    private bool isAttacking = false;
    bool toggleIsMoving = false;
    SpriteResolver houndsharkSpriteResolver;
    string currentDirection;
    readonly string[] labels = { "1", "2", "3", "4" };
    float animTimer = 0.0f;
    int animFrame = 0;

    Transform playerTransform;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        houndsharkSpriteResolver = GetComponent<SpriteResolver>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float animSpeed = isMoving ? swimAnimSpeedMoving : swimAnimSpeedIdle;
        animTimer += Time.fixedDeltaTime;

        if (animTimer >= animSpeed)
        {
            animTimer = 0f;
            animFrame = (animFrame + 1) % labels.Length;
        }

        // Set Angle of Sprite
        if (isChasing || isAttacking)
        {
            Vector2 direction = playerTransform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Debug.Log(direction.x);
            // Left sprite (default facing left)
            if (direction.x < 0)
            {
                currentDirection = "Left";
                transform.rotation = Quaternion.Euler(0, 0, angle + 180f);
            }

            // Right sprite
            if (direction.x > 0)
            {
                currentDirection = "Right";
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
        else if (isIdle)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime);
        }
        
        // Apply sprite
        houndsharkSpriteResolver.SetCategoryAndLabel(currentDirection, labels[animFrame]);
    }

    public void ToggleIsMoving(bool toggle)
    {
        isMoving = toggle;
    }

    public void ToggleIsChasing(bool toggle)
    {
        isChasing = toggle;
    }

    public void ToggleIsIdle(bool toggle)
    {
        isIdle = toggle;
    }

    public void ToggleIsAttacking(bool toggle)
    {
        isAttacking = toggle;
    }

    public void SetCurrentDirection(string direction)
    {
        currentDirection = direction;
    }
}
