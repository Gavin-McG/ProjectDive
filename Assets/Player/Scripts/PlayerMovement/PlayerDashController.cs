using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDashController : MonoBehaviour
{
    [SerializeField] InputActionReference moveActionReference;
    [SerializeField] InputActionReference dashActionReference;
    [SerializeField] float dashForce = 1.5f;
    [SerializeField] private float timeCooldown = 2.0f;
    

    private Rigidbody2D rb;
    private bool canDash;
    private float timeDuration;
    private Vector2 lastInputMove;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        canDash = true;
        timeDuration = 0f;
        lastInputMove = Vector2.zero;
    }
    
    private void FixedUpdate()
    {
        float inputDash = dashActionReference.action.ReadValue<float>();
        
        Vector2 inputMove = moveActionReference.action.ReadValue<Vector2>();
        inputMove.Normalize();

        Vector2 dashVector;
        if (inputMove == Vector2.zero) dashVector = dashForce * lastInputMove.normalized;
        else dashVector = dashForce * inputMove.normalized;
        
        if (inputDash >= 1 && canDash)
        {
            rb.AddForce(dashVector, ForceMode2D.Force);
            canDash = false;
        }

        if (!canDash)
        {
            timeDuration += Time.deltaTime;
            if (timeDuration >= timeCooldown)
            {
                timeDuration = 0;
                canDash = true;
            }
        }

        if (inputMove != Vector2.zero) lastInputMove = inputMove;
    }
}
