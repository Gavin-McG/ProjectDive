using System;
using UnityEngine;

public class FlipToFace : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float threshold = 1;

    private void Update()
    {
        if (transform.position.x < target.position.x - threshold)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (transform.position.x > target.position.x + threshold)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
