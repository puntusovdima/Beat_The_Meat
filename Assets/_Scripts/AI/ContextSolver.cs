using System;
using System.Collections.Generic;
using UnityEngine;

public class ContextSolver : MonoBehaviour
{
    [SerializeField] private bool showGizmos = true;
        
    // gizmo parameters
    float[] interestGizmo = new float[0];
    Vector2 resultDirection = Vector2.zero;
    private float rayLength = 1f;

    private void Start()
    {
        interestGizmo = new float[8];
    }

    public Vector2 GetDirectionToMove(List<SteeringBehaviour> behaviours, AIData aiData)
    {
        float[] danger = new float[8];
        float[] interest = new float[8];
        
        foreach (SteeringBehaviour behaviour in behaviours)
        {
            (danger, interest) = behaviour.GetSteering(danger, interest, aiData);
        }
        
        // Substract danger values from interest array
        for (int i = 0; i < interest.Length; i++)
        {
            interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
        }

        interestGizmo = interest;
        
        // get the average direction
        Vector2 outputDirection = Vector2.zero;
        for (int i = 0; i < interest.Length; i++)
        {
            outputDirection += Directions.eightDirections[i] * interest[i];
        }
        outputDirection.Normalize();

        resultDirection = outputDirection;
        
        // return the selected movement direction
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
