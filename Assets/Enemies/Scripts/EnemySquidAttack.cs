using UnityEngine;

public class EnemySquidAttack : MonoBehaviour
{
    [SerializeField] GameObject ink;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void SpawnInk()
    {
        GameObject inkObject = Instantiate(ink, transform.position, Quaternion.identity);
        inkObject.SetActive(true);
        Destroy(inkObject, 8.0f);
    }
}
