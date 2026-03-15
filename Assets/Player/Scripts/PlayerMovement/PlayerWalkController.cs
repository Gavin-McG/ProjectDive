using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerWalkController : MonoBehaviour
{
    [SerializeField] InputActionReference moveActionReference;
    [SerializeField] float walkSpeed = 1.5f;

    private Rigidbody2D rb;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 input = moveActionReference.action.ReadValue<Vector2>();
        input.y = 0;
        input.Normalize();
        
        //Get normal for floor underneath player (default to Vector2.up)

        //Apply input to player
        rb.linearVelocity = input.x * walkSpeed * Vector3.right;
    }
}
