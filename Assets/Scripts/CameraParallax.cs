using System;
using Unity.Mathematics;
using UnityEngine;

public class CameraParallax : MonoBehaviour
{
    [SerializeField] private float parallaxFactor = 0f;
    [SerializeField] private bool3 axis = new bool3(true, true, false);

    private Camera cam;
    private Vector3 initialPosition;
    private Vector3 initialCameraPosition;

    private Vector3 GetPosition()
    {
        return transform.localPosition;
    }

    private void SetPosition(Vector3 position)
    {
        transform.localPosition = position;
    }

    private Vector3 GetCameraPosition()
    {
        return cam?.transform.position ?? Vector3.zero;
    }

    private void OnEnable()
    {
        cam = Camera.main;
        initialPosition = transform.localPosition;
        initialCameraPosition = GetCameraPosition();
    }

    private void OnDisable()
    {
        SetPosition(initialPosition);
    }

    private void Update()
    {
        Vector3 cameraPosition = GetCameraPosition();
        Vector3 parallaxPosition = (cameraPosition - initialCameraPosition) * parallaxFactor;
        Vector3 newPosition = new Vector3(
            axis.x ? initialPosition.x + parallaxPosition.x : initialPosition.x,
            axis.y ? initialPosition.y + parallaxPosition.y : initialPosition.y,
            axis.z ? initialPosition.z + parallaxPosition.z : initialPosition.z
        );
        SetPosition(newPosition);
    }
}