using System.Collections;
using UnityEngine;


public class EnemyBeatController : CharacterBeatController, ITriggerEnter 
{
    private enum CharacterState { Chase, Attack, Hurt, WaitToAttack, Death, AvoidObstacle }

    [SerializeField] private float minTimeBeforeAttack, minDistanceToAttack, maxTimeBeforeAttack;
    [SerializeField] private float obstacleDetectionRadius = 1f;
    [SerializeField] private LayerMask obstacleLayer; // Set this in inspector to detect walls/obstacles
    [SerializeField] private float avoidanceHeight = 3f; // How high the enemy should try to move to avoid obstacles
    
    private CharacterState _state;
    private Vector2 _avoidanceTarget;
    private bool _isAvoidingObstacle;
    private float _originalGravityScale;
    
    private readonly int _idleAnimState = Animator.StringToHash("Enemy_Idle");
    private readonly int _runAnimState = Animator.StringToHash("Enemy_Run");
    //private readonly int _jumpAnimState = Animator.StringToHash("Enemy_Jump");
    private readonly int _attackAnimState = Animator.StringToHash("Enemy_Attack");
    private readonly int _fallAnimState = Animator.StringToHash("Enemy_Fall");
    private readonly int _hitAnimState = Animator.StringToHash("Enemy_Hit");
    private readonly int _deathAnimState = Animator.StringToHash("Enemy_Death");

    private Rigidbody2D _rb;
    private Vector2 _movement;
    //private float _floorLevel;
    private Animator _anim;
    private Transform _target;
    [SerializeField] private Transform _player;
    [SerializeField] EnemyHealth _enemyHealth;

    private bool _canAttack;
    
   
    
    public void HitByPlayer(GameObject player)
    {
        //_enemyHealth.TakeDamage(player.GetComponent<PlayerBeatController>().damage);
        TakeHit(player.GetComponent<PlayerBeatController>().damage);
        Debug.Log("Enemy was hit by player");
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        _rb.gravityScale = 0;
        _state = CharacterState.Chase;
        //_floorLevel = float.MinValue;
        _canAttack = true;
        _target = _player;
    }
    
    private void Update()
    {
        //_target.position = _player.position;
        switch (_state)
        {
            case CharacterState.Chase:
                Chase();
                break;
            case CharacterState.AvoidObstacle:
                AvoidObstacle();
                break;
            case CharacterState.Attack:
                //Attack();
                break;
            case CharacterState.Hurt:
                Hurt();
                break;
            case CharacterState.WaitToAttack:
               // WaitToAttack();
                break;
            case CharacterState.Death:
               // Death();
                break;
        }
    }

    private void Hurt()
    {
        
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            return;
        
        if (_state == CharacterState.Chase)
        {
            CalculateAvoidancePath(collision.contacts[0].normal);
            _state = CharacterState.AvoidObstacle;
        }
    }

    private void CalculateAvoidancePath(Vector2 collisionNormal)
    {
        bool movingRight = transform.rotation.eulerAngles.y < 90;
        
        Vector2 currentPos = transform.position;
        _avoidanceTarget = new Vector2(
            currentPos.x + (movingRight ? obstacleDetectionRadius : -obstacleDetectionRadius),
            currentPos.y + avoidanceHeight
            );
        
        _isAvoidingObstacle = true;
    }

    private void AvoidObstacle()
    {
        if (!_isAvoidingObstacle)
        {
            _state = CharacterState.Chase;
            return;
        }
        
        Vector2 avoidanceDirection = (_avoidanceTarget - (Vector2) transform.position).normalized;
        _rb.velocity = new Vector2(avoidanceDirection.x * speedX, avoidanceDirection.y * speedY);
        
        // Check if we're close enough to the target to stop avoiding
        if (Vector2.Distance(transform.position, _avoidanceTarget) < 0.5f)
        {
            _isAvoidingObstacle = false;
            _state = CharacterState.Chase;
        }
        // Update Animation
        _anim.CrossFade(_runAnimState, 0.1f);
    }
    
    private void Chase()
    {
        Debug.Log("Enemy is chasing");
        if (!_target || _state == CharacterState.Hurt) return;
    
        _movement = _target.position - transform.position;
        _movement.Normalize();
        
        // Check for obstacles ahead
        Vector2 rayDirection = new Vector2(_movement.x, 0);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, obstacleDetectionRadius, obstacleLayer);
        
        if (hit.collider != null && hit.collider.gameObject != _target.gameObject)
        {
            CalculateAvoidancePath(hit.normal);
            _state = CharacterState.AvoidObstacle;
            return;
        }
        
        // Handle rotation and movement...
        _rb.velocity = new Vector2(_movement.x * speedX, _movement.y * speedY);
        if (_movement.x < 0 && !Mathf.Approximately(transform.rotation.y, 180))
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (_movement.x > 0 && !Mathf.Approximately(transform.rotation.y, 0))
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    
        _anim.CrossFade(_runAnimState, 0.1f);
    }
    
    public void TakeHit(float damageTaken)
    {
        if (_state == CharacterState.Attack) return;
        Debug.Log("Taking damage with: " + damageTaken + " damage");
        
        _enemyHealth.TakeDamage(damageTaken);

        if (_enemyHealth.currentHealth <= 0)
        {
            _currentHealthPoints = 0;
            _state = CharacterState.Death;
            StartCoroutine(HitAndDie());
            
        }
        else
        {
            _anim.CrossFadeInFixedTime(_hitAnimState, 0f);
            _state = CharacterState.Hurt;
            _rb.velocity = Vector2.zero;
            StartCoroutine(RecoverFromHit());
        }
    }
    
    private IEnumerator HitAndDie()
    {
        _anim.CrossFade(_hitAnimState, 0.1f);
        float animCrossFaid = 0.4f;
        yield return new WaitForSeconds(_anim.GetCurrentAnimatorStateInfo(0).length - animCrossFaid);
        _anim.CrossFadeInFixedTime(_deathAnimState, animCrossFaid);
        yield return new WaitForSeconds(_anim.GetCurrentAnimatorStateInfo(0).length + animCrossFaid);
        Destroy(gameObject);
        //_enemyHealth.Death();
    }

    private IEnumerator RecoverFromHit()
    {
        yield return new WaitForSeconds(0.2f);
        _state = CharacterState.Chase;
        _anim.Play(_runAnimState);
        //_anim.CrossFadeInFixedTime(_runAnimState, 0.2f);
    }


    

}
