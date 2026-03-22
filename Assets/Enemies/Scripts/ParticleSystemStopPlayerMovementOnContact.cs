using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParticleSystemStopPlayerMovementOnContact : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Drag Player Here")]
    [SerializeField] PlayerSwimController playerSwimController;

    [Header("Stun time")] 
    [SerializeField] float stunDuration = 1.0f;
    float stunTimer = 0.0f;
    bool stunned = false;

    [Header("Particle System Time - Edit in ps and here")] 
    [SerializeField] float particleLifeTime = 4.0f;
    float particleTimer = 0.0f;
    bool particleAlive = true;

    CircleCollider2D inkCollider;
    
    [Header("Size of Collider Change")]
    [SerializeField] AnimationCurve curve;
    private float initialRadius = 0.85f;
    private float finalRadius = 1.87f;
    
    List<ParticleSystem.Particle> enterList;
    void Start()
    {
        inkCollider = GetComponent<CircleCollider2D>();
        inkCollider.radius = initialRadius;
    }

    void Update()
    {
        if (stunTimer < stunDuration)
        {
            stunTimer += Time.deltaTime;
        }

        if (stunned && stunTimer >= stunDuration)
        {
            playerSwimController.canSwim = true;
            stunned = false;
        }

        if (particleTimer < particleLifeTime)
        {
            particleTimer += Time.deltaTime;
            float curveValue = curve.Evaluate(particleTimer / particleLifeTime);
            inkCollider.radius = Mathf.Lerp(initialRadius, finalRadius, curveValue);
        }
        else if (particleTimer >= particleLifeTime)
            particleAlive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && particleAlive)
        {
            stunned = true;
            playerSwimController.canSwim = false;
            stunTimer = 0.0f;
        }
    }
}
