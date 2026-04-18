using System;
using UnityEngine;

public class EnemyLeatherbackColliderManager : MonoBehaviour
{
    bool aiming = false;
    bool crushing = false;

    Transform playerTransform;

    EnemyLeatherbackAttack leatherbackAttack;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerTransform == null) playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        leatherbackAttack = transform.parent.GetComponent<EnemyLeatherbackAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        if (aiming)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y);
            playerTransform.position = new Vector2(playerTransform.position.x, playerTransform.position.y);
            Vector3 targetDirection = transform.position - playerTransform.position;
            Vector3 dir = targetDirection.normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    public void ToggleAiming(bool toggle)
    {
        aiming = toggle;
    }

    public void ToggleCrushing(bool toggle)
    {
        crushing = toggle;
    }

    public void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 90.0f);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && crushing)
        {
            Debug.Log("Crushing Player");
            leatherbackAttack.PlayerCrushingMovement();
        }
    }
}
