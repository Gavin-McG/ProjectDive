using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;

public class PlayerSwimSpriteController : MonoBehaviour
{
    [SerializeField] InputActionReference moveActionReference;
    [SerializeField] float swimAnimSpeedMoving = 0.15f;
    [SerializeField] float swimAnimSpeedIdle = 0.4f;

    private Rigidbody2D rb;
    private SpriteResolver playerSpriteResolver;

    private string currentDirection = "Right";
    private float animTimer = 0f;
    private int animFrame = 0;
    private readonly string[] labels = { "1", "2", "3", "4" };

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSpriteResolver = GetComponent<SpriteResolver>();
    }

    private void FixedUpdate()
    {
        Vector2 inputMove = moveActionReference.action.ReadValue<Vector2>();
        inputMove.Normalize();

        // Only change direction if moving left/right
        if (inputMove.x < 0)
            currentDirection = "Left";
        else if (inputMove.x > 0)
            currentDirection = "Right";

        bool isMoving = inputMove.magnitude > 0.01f;

        // Animation timing
        float animSpeed = isMoving ? swimAnimSpeedMoving : swimAnimSpeedIdle;
        animTimer += Time.fixedDeltaTime;

        if (animTimer >= animSpeed)
        {
            animTimer = 0f;
            animFrame = (animFrame + 1) % labels.Length;
        }

        // Apply sprite
        playerSpriteResolver.SetCategoryAndLabel(currentDirection, labels[animFrame]);
    }
}