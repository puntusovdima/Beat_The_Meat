using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public enum CharacterState { Dance, Runaway, Rage, Knockback}

    [SerializeField] private CharacterState state;
    [SerializeField] private float minDistanceToAttack;
    [SerializeField] private float detectionDelay = 0.01f, aiUpdateDelay = 0.06f, attackDelay = 0.5f;
    [SerializeField] private AIData aiData;
    [SerializeField] private List<SteeringBehaviour> steeringBehaviours;
    [SerializeField] List<Detector> detectors;
    [SerializeField] Vector2 movement;
    
    private readonly int _introAnimState = Animator.StringToHash("Boss_Intro");
    private readonly int _danceAnimState = Animator.StringToHash("Boss_Dance");
    [SerializeField] private ContextSolver movementDirectionSolver;

    [SerializeField] private float positionChangeThreshold = 0.1f;  // New: Minimum distance to move
    private Vector2 lastPosition;                                   // New: Store last position
    private float stuckTimer;                                      // New: Track how long we've been "stuck"
    [SerializeField] private float stuckThreshold = 0.5f;          // New: Time before considering new direction

    public Transform player;
    public bool isFlipped = false;

    private Rigidbody2D rb;
    private Animator _anim;
    
    [SerializeField] private float speed = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        rb.gravityScale = 0f;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastPosition = rb.position;  // New: Initialize last position
    }

    private void Start()
    {
        InvokeRepeating("PerformDetection", 0, detectionDelay);
        InvokeRepeating("UpdateBossStates", 0, aiUpdateDelay);
    }
    
    private void PerformDetection()
    {
        foreach (Detector detector in detectors)
        {
            detector.Detect(aiData);
        }
    }

    public void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

    private void UpdateBossStates()
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).shortNameHash == _introAnimState)
            return;
        LookAtPlayer();
        RunFromPlayer();
    }

    private void RunFromPlayer()
    {
        if (Vector2.Distance(transform.position, player.position) > minDistanceToAttack)
        {
            movement = Vector2.zero;
            rb.velocity = Vector2.zero;
            stuckTimer = 0f;  // Reset stuck timer when stopping
            return;
        }

        // Check if we've moved significantly
        float distanceMoved = Vector2.Distance(rb.position, lastPosition);
        
        if (distanceMoved < positionChangeThreshold)
        {
            stuckTimer += Time.deltaTime;
            
            // If we're stuck for too long, force a new direction
            if (stuckTimer >= stuckThreshold)
            {
                // Get a new direction but modify it slightly to break the deadlock
                movement = movementDirectionSolver.GetDirectionToMove(steeringBehaviours, aiData);
                movement = Quaternion.Euler(0, 0, Random.Range(-45f, 45f)) * movement;
                stuckTimer = 0f;
            }
        }
        else
        {
            // We've moved enough, update last position and reset timer
            lastPosition = rb.position;
            stuckTimer = 0f;
            movement = movementDirectionSolver.GetDirectionToMove(steeringBehaviours, aiData);
        }

        // Apply movement
        rb.velocity = new Vector2(movement.x * speed, movement.y * speed);
    }
}