using UnityEngine;

public class EnemyTriggersPhase : MonoBehaviour
{
    public static bool IsEnemyInRangeOfPlayer(Vector3 enemyPosition, Vector3 playerPosition, float attackDistance)
    {
        return Vector3.Distance(enemyPosition, playerPosition) < attackDistance;
    } 
}
