using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerSwimController : MonoBehaviour
{
    [SerializeField] InputActionReference moveActionReference;
    [SerializeField] float swimForce = 1.5f;
    [SerializeField] private float directionalFactor = 1.5f;

    private Rigidbody2D rb;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 input = moveActionReference.action.ReadValue<Vector2>();
        input.Normalize();
        
        Vector2 inputForce = swimForce * input.normalized;
        if (Vector2.Dot(inputForce, rb.linearVelocity) < 0)
        {
            inputForce *= directionalFactor;
        }
        rb.AddForce(inputForce, ForceMode2D.Force);
    }
}
