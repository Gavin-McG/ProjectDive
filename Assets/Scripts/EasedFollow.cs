using UnityEngine;

public class EasedFollow : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [Tooltip("1 -> Instant, 0 -> No Movement")]
    [SerializeField] private float easeFactor = 0.95f;

    private void LateUpdate()
    {
        Vector3 currentOffset = transform.position - target.transform.position;
        float easedOffsetPercent = Mathf.Pow(1-easeFactor, Time.deltaTime);
        Vector3 mewOffset = currentOffset * easedOffsetPercent;
        transform.position = target.transform.position + mewOffset;
    }
}
