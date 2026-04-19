using UnityEngine;
using UnityEngine.U2D.Animation;

public class EnemyLeatherbackSpriteManager : MonoBehaviour
{
    [SerializeField] float swimAnimSpeedMoving = 0.4f;
    [SerializeField] float swimAnimSpeedChasing = 0.15f;
    [SerializeField] float swimAnimSpeedIdle = 0.6f;
    
    SpriteResolver leatherbackSpriteResolver;
    Transform leatherbackTransform;
    EnemyLeatherbackManager leatherbackManager;
    EnemyLeatherbackAttack leatherbackAttack;
    Transform playerTransform;

    bool idle = true;
    bool chasing = false;
    bool aiming = false;
    bool crushing = false;
    
    private float animTimer = 0f;
    private int animFrame = 0;
    private string currentDirection = "Left";
    private readonly string[] labels = { "1", "2", "3", "4" };

    Quaternion initRotation;
    
    int animCrushFrame = 0;
    string[] crushLabels =
    {
        "0", "1", "2", "3", "4"
    };

    Vector2 prevPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leatherbackSpriteResolver = GetComponent<SpriteResolver>();
        leatherbackTransform = transform.parent.transform;
        leatherbackManager = transform.parent.GetComponent<EnemyLeatherbackManager>();
        leatherbackAttack = transform.parent.GetComponent<EnemyLeatherbackAttack>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        prevPosition = leatherbackTransform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DetermineLeatherbackPhase();
        if (idle)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime);
            
            bool isMoving = Vector3.Distance(prevPosition, leatherbackTransform.position) >= 0.01f;
            currentDirection = leatherbackTransform.position.x - prevPosition.x > 0 ? "Right" : "Left";
            
            // Animation timing
            float animSpeed = isMoving ? swimAnimSpeedMoving : swimAnimSpeedIdle;
            animTimer += Time.fixedDeltaTime;
            if (animTimer >= animSpeed)
            {
                animTimer = 0f;
                animFrame = (animFrame + 1) % labels.Length;
            }
            
            leatherbackSpriteResolver.SetCategoryAndLabel(currentDirection, labels[animFrame]);
        }
        
        else if (chasing)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime);
            currentDirection =  playerTransform.position.x - leatherbackTransform.position.x > 0 ? "Right" : "Left";
            
            // Animation timing
            float animSpeed = swimAnimSpeedChasing;
            animTimer += Time.fixedDeltaTime;
            Debug.Log("animTimer: " + animTimer);
            if (animTimer >= animSpeed)
            {
                
                animTimer = 0f;
                animFrame = (animFrame + 1) % labels.Length;
            }
            
            leatherbackSpriteResolver.SetCategoryAndLabel(currentDirection, labels[animFrame]);
        }
        else if (aiming && !crushing)
        {
            if (leatherbackAttack.GetChargingProgress() <= 0.05f)
            {
                initRotation = transform.rotation;
            }
            // Look at player
            Vector2 direction = transform.position - playerTransform.position;
            Vector2 dir = direction.normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            
            if (currentDirection == "Left")
                transform.rotation = Quaternion.Lerp(initRotation, Quaternion.Euler(0f, 0f, angle), leatherbackAttack.GetChargingProgress());
            else 
                transform.rotation = Quaternion.Lerp(initRotation, Quaternion.Euler(0f, 0f, angle + 180.0f), leatherbackAttack.GetChargingProgress());
            
            // Animation timing
            float animSpeed = swimAnimSpeedMoving;
            animTimer += Time.fixedDeltaTime;
            Debug.Log("animTimer: " + animTimer);
            if (animTimer >= animSpeed)
            {
                animTimer = 0f;
                animFrame = (animFrame + 1) % labels.Length;
            }
            
            leatherbackSpriteResolver.SetCategoryAndLabel(currentDirection, labels[animFrame]);
        }
        else if (crushing && !aiming)
        {
            float progress = leatherbackAttack.GetCrushingProgress();
            if (progress <= 0.05f)
            {
                if (currentDirection == "Right")
                {
                    Debug.Log("To the right");
                    for (int i = 0; i < crushLabels.Length; ++i)
                    {
                        crushLabels[i] = (i + 5).ToString();
                    }
                }
                else
                {
                    for (int i = 0; i < crushLabels.Length; ++i)
                    {
                        crushLabels[i] = (i).ToString();
                    }
                }
            }

            //Crushing animation
            Debug.Log(progress);
            
            if (progress > 0.0f & progress <= 0.2f) animCrushFrame = 0;
            if (progress > 0.2f & progress <= 0.4f) animCrushFrame = 1;
            if (progress > 0.4f & progress <= 0.6f) animCrushFrame = 2;
            if (progress > 0.6f & progress <= 0.8f) animCrushFrame = 3;
            if (progress > 0.8f) animCrushFrame = 4;
            
            leatherbackSpriteResolver.SetCategoryAndLabel("Crush", crushLabels[animCrushFrame]);
        }
        
        prevPosition = leatherbackTransform.position;
    }

    void DetermineLeatherbackPhase()
    {
        if (leatherbackManager.GetIdle())
        {
            idle = true;
            chasing = false;
            aiming = false;
            crushing = false;
        }
        else if (leatherbackManager.GetChasing())
        {
            idle = false;
            chasing = true;
            aiming = false;
            crushing = false;
        }
        else if (leatherbackManager.GetAttacking())
        {
            if (leatherbackAttack.GetCharging())
            {
                idle = false;
                chasing = false;
                aiming = true;
                crushing = false;
            }
            else if (leatherbackAttack.GetCrushing())
            {
                idle = false;
                chasing = false;
                aiming = false;
                crushing = true;
            }
            else
            {
                idle = false;
                chasing = true;
                aiming = false;
                crushing = false;
            }
        }
    }
}
