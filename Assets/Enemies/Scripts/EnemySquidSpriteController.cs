using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class EnemySquidSpriteController : MonoBehaviour
{
    [SerializeField]  private float swimAnimSpeedMoving = 0.15f;
    [SerializeField] private float swimAnimSpeedIdle = 0.4f;
    [SerializeField] private float swinAnimDurationMoving = 0.5f;

    bool isMoving = false;
    bool toggleIsMoving = false;
    SpriteResolver squidSpriteResolver;
    string currentDirection;
    readonly string[] labels = { "1", "2", "3", "4" };
    float animTimer = 0.0f;
    int animFrame = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        squidSpriteResolver = GetComponent<SpriteResolver>();
        currentDirection = "Left";
        StartCoroutine(BurstMovementEvent());
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

        // Apply sprite
        squidSpriteResolver.SetCategoryAndLabel(currentDirection, labels[animFrame]);
    }

    IEnumerator BurstMovementEvent()
    {
        while (true)
        {
            if (toggleIsMoving)
            {
                StartCoroutine(HandleIsMoving());
            }
            yield return null;
        }
    }

    IEnumerator HandleIsMoving()
    {
        isMoving = true;
        yield return new WaitForSeconds(swinAnimDurationMoving);
        isMoving = false;
    }

    public void ToggleIsMoving()
    {
        toggleIsMoving = true;
    }

    public void DetermineDirection(float horizontalMovement)
    {
        currentDirection = horizontalMovement > 0 ? "Right" : "Left";
    }
}
