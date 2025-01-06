using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextSolver : MonoBehaviour
{
    [SerializeField]
    private bool showGizmos = true;
    //gizmo parameters
    float[] interestGizmo = new float[0];
    Vector2 resultDirection = Vector2.zero;
    private float rayLength = 2;

    [SerializeField]
    private float obstacleAvoidanceWeight = 2f;    
    [SerializeField]
    private float playerAvoidanceWeight = 1.5f;    

    [SerializeField]
    private float directionChangeThreshold = 0.2f;  // New: Minimum difference needed to change direction
    private Vector2 currentDirection;               // New: Store the current movement direction

    private void Start()
    {
        interestGizmo = new float[8];
        currentDirection = Vector2.zero;
    }

    public Vector2 GetDirectionToMove(List<SteeringBehaviour> behaviours, AIData aiData)
    {
        float[] danger = new float[8];
        float[] interest = new float[8];

        //Loop through each behaviour
        foreach (SteeringBehaviour behaviour in behaviours)
        {
            (danger, interest) = behaviour.GetSteering(danger, interest, aiData);
        }

        if (gameObject.CompareTag("Dancer"))
        {
            float[] finalInterest = new float[8];
            
            // Apply thresholds to danger and interest values
            for (int i = 0; i < 8; i++)
            {
                // Only consider danger values above the threshold
                float dangerValue = danger[i] > directionChangeThreshold ? danger[i] : 0;
                float interestValue = Mathf.Abs(interest[i]) > directionChangeThreshold ? interest[i] : 0;

                if (dangerValue > 0)
                {
                    finalInterest[i] = -dangerValue * obstacleAvoidanceWeight;
                }
                else
                {
                    finalInterest[i] = -interestValue * playerAvoidanceWeight;
                }
            }
            interest = finalInterest;
        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
            }
        }

        Vector2 outputDirection = Vector2.zero;
        for (int i = 0; i < 8; i++)
        {
            outputDirection += Directions.eightDirections[i] * interest[i];
        }
        
        // Only update direction if the change is significant
        if (outputDirection.magnitude > directionChangeThreshold)
        {
            outputDirection.Normalize();
            
            // Only change direction if the new direction is significantly different
            if (currentDirection == Vector2.zero || 
                Vector2.Distance(outputDirection, currentDirection) > directionChangeThreshold)
            {
                currentDirection = outputDirection;
            }
        }

        resultDirection = currentDirection;
        return resultDirection;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && showGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, resultDirection * rayLength);
        }
    }
}