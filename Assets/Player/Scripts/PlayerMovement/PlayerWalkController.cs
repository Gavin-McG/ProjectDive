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
        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = LayerMask.GetMask("Ground");
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        Physics2D.Raycast(transform.position, Vector2.down, filter, hits, 5f);
        Vector2 upDir = hits.FirstOrDefault().normal;
        if (upDir == Vector2.zero) upDir = Vector2.up;

        //Apply input to player
        Vector2 walkDir = upDir.Perpendicular1();
        rb.linearVelocity = input.x * walkSpeed * walkDir;
    }
}
