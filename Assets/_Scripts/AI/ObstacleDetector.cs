using UnityEngine;
using UnityEngine.Serialization;

public class ObstacleDetector : Detector
{
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private LayerMask obstacleLayerMask
        ;
    [SerializeField] private bool showGizmos = true;
    private Collider2D[] colliders;
    
    public override void Detect(AIData aiData)
    {
        colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, obstacleLayerMask);
        aiData.obstacles = colliders;
    }

    private void OnDrawGizmos()
    {
        if (showGizmos == false) return;
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (Application.isPlaying && colliders != null)
        {
            Gizmos.color = Color.red;
            foreach (Collider2D obstacleCollider in colliders)
            {
                Gizmos.DrawSphere(obstacleCollider.transform.position, 0.2f);
            }
        }
    }
}
