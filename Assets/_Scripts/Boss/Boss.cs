using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : MonoBehaviour, ITriggerEnter
{
    public enum CharacterState { Idle, RunAway, Dance, Rage, Knockback }

    [SerializeField] public CharacterState currentState;
    [SerializeField] private float minDistanceToAttack;
    [SerializeField] private float detectionDelay = 0.01f, aiUpdateDelay = 0.06f;
    [SerializeField] private AIData aiData;
    [SerializeField] private List<SteeringBehaviour> steeringBehaviours;
    [SerializeField] private List<Detector> detectors;
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private ContextSolver movementDirectionSolver;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float positionChangeThreshold = 0.1f;
    [SerializeField] private float stuckThreshold = 0.5f;

    [SerializeField] private GameObject youWinUI;
    
    private readonly int _introAnimState = Animator.StringToHash("Boss_Intro");

    private Rigidbody2D rb;
    private Animator _anim;
    private Transform player;

    private Vector2 movement;
    private bool isFlipped = false;
    private Vector2 lastPosition;
    private float stuckTimer;
    private CharacterState previousState;
    private bool isTakingHit = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        rb.gravityScale = 0f; // Prevent falling in a 2D plane
        currentState = CharacterState.Idle;
        lastPosition = rb.position;
    }

    private void Start()
    {
        InvokeRepeating(nameof(PerformDetection), 0, detectionDelay);
        InvokeRepeating("UpdateBossStates", 0, aiUpdateDelay);
    }

    private void UpdateBossStates()
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).shortNameHash == _introAnimState)
            return;
        if (!isTakingHit)
        {
            switch (currentState)
            {
                case CharacterState.Idle:
                    HandleIdle();
                    break;
                case CharacterState.RunAway:
                    HandleRunAway();
                    break;
                case CharacterState.Knockback:
                    // Knockback handled in TakeHit coroutine
                    break;
                // Add other states like Dance, Rage here as needed
            }
        }
    }

    private void PerformDetection()
    {
        foreach (var detector in detectors)
        {
            detector.Detect(aiData);
        }
    }

    private void HandleIdle()
    {

        LookAtPlayer();

        if (Vector2.Distance(transform.position, player.position) < minDistanceToAttack)
        {
            SwitchState(CharacterState.RunAway);
        }
    }

    private void HandleRunAway()
    {
        LookAtPlayer();

        if (Vector2.Distance(transform.position, player.position) > minDistanceToAttack)
        {
            rb.velocity = Vector2.zero;
            stuckTimer = 0f;
            GetComponent<HeadSpawner>().enabled = true; // Enable HeadSpawner
            SwitchState(CharacterState.Idle);
            return;
        }
        // GetComponent<HeadSpawner>().enabled = false; // Disable HeadSpawner when moving

        // float distanceMoved = Vector2.Distance(rb.position, lastPosition);
        // if (distanceMoved < positionChangeThreshold)
        // {
        //     stuckTimer += Time.deltaTime;
        //     if (stuckTimer >= stuckThreshold)
        //     {
        //         // Modify direction slightly to break the deadlock
        //         movement = movementDirectionSolver.GetDirectionToMove(steeringBehaviours, aiData);
        //         movement = Quaternion.Euler(0, 0, Random.Range(-45f, 45f)) * movement;
        //         stuckTimer = 0f;
        //     }
        // }
        // else
        // {
        //     lastPosition = rb.position;
        //     stuckTimer = 0f;
        //     movement = movementDirectionSolver.GetDirectionToMove(steeringBehaviours, aiData);
        // }
        movement = movementDirectionSolver.GetDirectionToMove(steeringBehaviours, aiData);


        rb.velocity = movement * speed;
    }

    public void HitByPlayer(GameObject player)
    {
        gameObject.GetComponent<HeadSpawner>().SpawnEnemiesWhenHit();
        TakeHit(player.GetComponent<PlayerBeatController>().damage);
    }

    public void HitByEnemy(GameObject enemy)
    {
        throw new System.NotImplementedException();
    }

    private void TakeHit(float damage)
    {
        if (isTakingHit) return;

        enemyHealth.TakeDamage(damage);
        Debug.Log($"Boss is HitByPlayer, boss Health: {enemyHealth.currentHealth}");

        if (enemyHealth.currentHealth <= 0)
        {
            youWinUI.SetActive(true);
            Debug.Log("YOU WIN YOU WIN YOU WIN YOU WIN YOU WIN YOU WIN YOU WIN");
            Destroy(gameObject);
            return;
        }

        StartCoroutine(HandleHitEffect());
    }

    private IEnumerator HandleHitEffect()
    {
        isTakingHit = true;
        previousState = currentState;
        SwitchState(CharacterState.Knockback);

        // Freeze movement
        rb.velocity = Vector2.zero;

        // Flash white effect
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.2f);

        spriteRenderer.color = originalColor;

        yield return new WaitForSeconds(0.5f);

        isTakingHit = false;
        SwitchState(previousState);
    }

    private void LookAtPlayer()
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

    private void SwitchState(CharacterState newState)
    {
        currentState = newState;
    }
}
