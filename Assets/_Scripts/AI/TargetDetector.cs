using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : Detector
{
    [SerializeField] private float targetDetectionRadius = 5f;
    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private bool showGizmos = false;
    
    private List<Transform> colliders;
    
    public override void Detect(AIData aiData)
    {   
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, targetDetectionRadius, playerLayerMask);

        if (playerCollider != null)
        {
            Vector2 direction = (playerCollider.transform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, targetDetectionRadius, obstaclesLayerMask);
            
            // Make sure that the collider we see is on the Player layer
            if (hit.collider != null && (playerLayerMask & (1 << hit.collider.gameObject.layer)) != 0)
            {
                Debug.DrawRay(transform.position, direction * targetDetectionRadius, Color.magenta);
                colliders = new List<Transform>() { playerCollider.transform };
            }
            else
            {
                colliders = null;
            }
        }
        else
        {
            colliders = null;
        }
        aiData.targets = colliders;
    }

    private void OnDrawGizmos()
    {
        if (showGizmos == false) return;

        Gizmos.DrawWireSphere(transform.position, targetDetectionRadius);
        
        if (colliders == null) return;
        Gizmos.color = Color.magenta;
        foreach (var item in colliders)
        {
            Gizmos.DrawSphere(item.position, 0.3f);
        }
    }
}
