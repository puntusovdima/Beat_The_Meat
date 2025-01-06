using UnityEngine;

public class ExplodingHead : MonoBehaviour
{
    private Vector2 targetPosition; // The player's position
    private Vector2 startPosition; // The initial position of the bomb
    private float launchHeight = 5f; // Height of the parabolic arc
    private float speed = 5f; // Speed of the bomb

    [SerializeField] private float damage = 10f;

    private float timeElapsed;
    private float totalTravelTime;

    public void SetTarget(Vector2 playerPosition)
    {
        targetPosition = playerPosition;
        startPosition = transform.position;

        // Calculate total travel time based on the distance and speed
        totalTravelTime = Vector2.Distance(startPosition, targetPosition) / speed;
    }

    private void Update()
    {
        if (totalTravelTime > 0)
        {
            FlyInParabolicArc();
        }
    }

    private void FlyInParabolicArc()
    {
        // Update the elapsed time
        timeElapsed += Time.deltaTime;

        // Calculate the linear interpolation (Lerp) factor
        float t = timeElapsed / totalTravelTime;

        // Lerp between the start and target positions
        Vector2 currentPosition = Vector2.Lerp(startPosition, targetPosition, t);

        // Add the parabolic height
        float parabolicHeight = launchHeight * 4 * (t * (1 - t)); // Parabolic equation
        currentPosition.y += parabolicHeight;

        // Update the bomb's position
        transform.position = currentPosition;

        // Destroy the bomb when it reaches the target
        if (t >= 1.05f)
        {
            
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponent<PlayerBeatController>().TakeHit(damage);
    }
}