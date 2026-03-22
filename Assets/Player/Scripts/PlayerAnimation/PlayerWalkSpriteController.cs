using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;

public class PlayerWalkSpriteController : MonoBehaviour
{
    [SerializeField] InputActionReference moveActionReference;
    [SerializeField] float walkAnimSpeedMoving = 0.2f;

    private SpriteResolver playerSpriteResolver;

    private string currentDirection = "Right";
    private float animTimer = 0f;
    private int animFrame = 0;
    private readonly string[] labels = { "1", "2", "3", "4" };

    void Start()
    {
        playerSpriteResolver = GetComponent<SpriteResolver>();
    }

    private void FixedUpdate()
    {
        Vector2 inputMove = moveActionReference.action.ReadValue<Vector2>();
        inputMove.Normalize();

        bool isMoving = inputMove.magnitude > 0.01f;

        // Only change direction if moving left/right
        if (inputMove.x < 0)
            currentDirection = "Left";
        else if (inputMove.x > 0)
            currentDirection = "Right";

        if (isMoving)
        {
            // Animate walk
            animTimer += Time.fixedDeltaTime;
            if (animTimer >= walkAnimSpeedMoving)
            {
                animTimer = 0f;
                animFrame = (animFrame + 1) % labels.Length;
            }

            playerSpriteResolver.SetCategoryAndLabel(currentDirection, labels[animFrame]);
        }
        else
        {
            // Idle sprite
            playerSpriteResolver.SetCategoryAndLabel("Idle", "1");
        }
    }
}