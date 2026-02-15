using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;

public class PlayerSpriteMovement : MonoBehaviour
{
    [SerializeField] InputActionReference moveActionReference;
    
    private Rigidbody2D rb;
    private SpriteResolver playerSpriteResolver;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSpriteResolver = GetComponent<SpriteResolver>();

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector2 inputMove = moveActionReference.action.ReadValue<Vector2>();
        inputMove.Normalize();

        if (inputMove.x < 0)
            playerSpriteResolver.SetCategoryAndLabel("Left", "Sprite1");
        else if (inputMove.x > 0)
            playerSpriteResolver.SetCategoryAndLabel("Right", "Sprite1");
        else if (inputMove.y > 0)
            playerSpriteResolver.SetCategoryAndLabel("Up", "Sprite1");
        else if (inputMove.y < 0)
            playerSpriteResolver.SetCategoryAndLabel("Down", "Sprite1");
        else 
            playerSpriteResolver.SetCategoryAndLabel("Idle", "Sprite1");
    }
}
