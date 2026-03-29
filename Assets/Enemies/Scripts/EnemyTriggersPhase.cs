using UnityEngine;

public class EnemyTriggersPhase : MonoBehaviour
{
    public static bool IsEnemyInRangeOfPlayer(Vector3 enemyPosition, Vector3 playerPosition, float attackDistance)
    {
        return Vector3.Distance(enemyPosition, playerPosition) < attackDistance;
    }

    public static Vector2 FindNearestWallPoint(Transform player, float searchRadius)
    {
        LayerMask wallLayerMask = LayerMask.GetMask("Lighting-Tiles"); 
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(player.position, searchRadius, wallLayerMask);

        Collider2D closest = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            Vector2 closestPoint = hit.ClosestPoint(player.position);
            float dist = Vector2.Distance(player.position, closestPoint);

            if (dist < minDist)
            {
                minDist = dist;
                closest = hit;
            }
        }

        if (closest != null)
        {
            Vector2 nearestWallPoint = closest.ClosestPoint(player.position);
            return nearestWallPoint;
        }
        return Vector2.negativeInfinity;
    }
}
