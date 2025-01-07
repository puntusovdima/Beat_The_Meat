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
    private float obstacleAvoidanceWeight = 2f;    // Weight for obstacle avoidance
    [SerializeField]
    private float playerAvoidanceWeight = 1.5f;    // Weight for running away from player

    private void Start()
    {
        interestGizmo = new float[8];
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
            // Handle obstacle avoidance first
            float[] finalInterest = new float[8];
            for (int i = 0; i < 8; i++)
            {
                // Strongly avoid directions with danger (obstacles)
                if (danger[i] > 0)
                {
                    finalInterest[i] = -danger[i] * obstacleAvoidanceWeight;
                }
                else
                {
                    // For directions without immediate danger, run away from player
                    // Invert the interest (which should contain player position info)
                    // Higher interest means we want to go in the opposite direction
                    finalInterest[i] = -interest[i] * playerAvoidanceWeight;
                }
            }
            interest = finalInterest;
        }
        else
        {
            //subtract danger values from interest array for non-dancer objects
            for (int i = 0; i < 8; i++)
            {
                interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
            }
        }

        //get the average direction
        Vector2 outputDirection = Vector2.zero;
        for (int i = 0; i < 8; i++)
        {
            outputDirection += Directions.eightDirections[i] * interest[i];
        }
        outputDirection.Normalize();
        resultDirection = outputDirection;

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