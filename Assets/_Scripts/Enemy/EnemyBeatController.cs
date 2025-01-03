using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


public class EnemyBeatController : CharacterBeatController, ITriggerEnter 
{
    public enum CharacterState { Chase, Attack, Hurt, WaitToAttack, Death, Idle, Knockback }

    // later attack delay should be randomly chosen(or clamped) between minTimeBeforeAttack and maxTimeBeforeAttack
    [SerializeField] private float minTimeBeforeAttack, minDistanceToAttack, maxTimeBeforeAttack;
    // [SerializeField] private Transform attackPoint;
    // AI detection stuff
    [SerializeField]
    private float detectionDelay = 0.01f, aiUpdateDelay = 0.06f, attackDelay = 1f;
    [SerializeField] private AIData aiData;
    [SerializeField] private List<SteeringBehaviour> steeringBehaviours;
    [SerializeField] List<Detector> detectors;
    // private float attackDistance = 0.5f;

    [SerializeField] private Vector2 movementInput;
    
    [SerializeField] private ContextSolver movementDirectionSolver;

    
    private CharacterState _state;
    
    private readonly int _idleAnimState = Animator.StringToHash("Enemy_Idle");
    private readonly int _runAnimState = Animator.StringToHash("Enemy_Run");
    private readonly int _attackAnimState1 = Animator.StringToHash("Enemy_Attack1");
    private readonly int _fallAnimState = Animator.StringToHash("Enemy_Fall");
    private readonly int _hitAnimState = Animator.StringToHash("Enemy_Hit");
    private readonly int _deathAnimState = Animator.StringToHash("Enemy_Death");
    private readonly int _blockAnimState = Animator.StringToHash("Enemy_Block");

    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Animator _anim;
    [SerializeField] Transform target;
    // [SerializeField] private Transform _player;
    [SerializeField] EnemyHealth enemyHealth;
    private bool _canAttack = true;

    public UnityEvent enemyHit;
    

    
   
    public void SetState(CharacterState state)
    {
        // Debug.Log($"State changed to: {state}");
        // _state = CharacterState.Knockback;
        _state = state;
    }
    public void HitByPlayer(GameObject player)
    {
        enemyHit.Invoke();
        //_enemyHealth.TakeDamage(player.GetComponent<PlayerBeatController>().damage);
        TakeHit(player.GetComponent<PlayerBeatController>().damage);
        Debug.Log("Enemy was hit by player");
    }

    public void HitByEnemy(GameObject enemy)
    {
        // throw new System.NotImplementedException();
        Debug.Log("Am I hitting myself?(Enemy)");
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        _rb.gravityScale = 0;
        _state = CharacterState.Chase;
        //_floorLevel = float.MinValue;
        _canAttack = true;
        // _target = _player;
    }
    
    private void Start()
    {
        InvokeRepeating("PerformDetection", 0, detectionDelay);
        InvokeRepeating("UpdateEnemyStates", 0, aiUpdateDelay);
     }
    private void PerformDetection()
    {
        foreach (Detector detector in detectors)
        {
            detector.Detect(aiData);
        }
    }
    
    private void UpdateEnemyStates()
    {
        switch (_state)
        {
            case CharacterState.Idle:
                Idle();
                break;
            case CharacterState.Chase:
                Chase();
                break;
            case CharacterState.Attack:
                Attack();
                break;
            case CharacterState.Hurt:
                //Hurt();
                break;
            case CharacterState.WaitToAttack:
               WaitToAttack();
                break;
            case CharacterState.Death:
               // Death();
                break;
            case CharacterState.Knockback:
                Knockback();
                break;
        }
    }

    private void WaitToAttack()
    {
        Debug.Log("Enemy is waiting to attack");
        _state = CharacterState.Attack;
        StartCoroutine(AttackWithDelay());
    }
    
    private IEnumerator AttackWithDelay()
    {
        yield return new WaitForSeconds(Random.Range(minTimeBeforeAttack, maxTimeBeforeAttack));
        Attack();
    }

    private void Attack()
    {
        if (!_canAttack) return;
        Debug.Log("Enemy is attacking player");
        Collider2D[] results = Physics2D.OverlapBoxAll(hitAnchor.position, hitSize, 0);
        _canAttack = false;
        _rb.velocity = Vector2.zero;
        // _anim.CrossFadeInFixedTime(_attackAnimState, 0.2f);
        _anim.Play(_attackAnimState1);
        foreach (Collider2D result in results)
        {
            if ((1 << result.gameObject.layer) == hitLayerMask.value)
            {
                result.GetComponent<ITriggerEnter>()?.HitByEnemy(gameObject);
            }
            Debug.Log("Result: " + result.gameObject.name);
        }

        StartCoroutine(WaitForAttackAnimationToEnd(_anim.GetCurrentAnimatorStateInfo(0)));
    }

    private IEnumerator WaitForAttackAnimationToEnd(AnimatorStateInfo stateInfo)
    {
        while (stateInfo.shortNameHash != _attackAnimState1)
        {
            yield return null;
            stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        }

        while (stateInfo.normalizedTime < 1f)
        {
            yield return null;
            stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        }
        _canAttack = true;
        _state = CharacterState.Idle;
        _anim.CrossFadeInFixedTime(_idleAnimState, 0f);
    }

    private void Knockback()
    {
        
        Debug.Log("Enemy is in the knockback state(EnemyBeatController)");
    }


    private void Idle()
    {
        if (Vector2.Distance(transform.position, target.position) < minDistanceToAttack)
        {
            _state = CharacterState.Attack;
        }
        if (aiData.currentTarget || aiData.GetTargetsCount() > 0)
        {
            Chase();
            _state = CharacterState.Chase;
            return;
        }
        Debug.Log("Enemy is idling");
        _rb.velocity = Vector2.zero; // Stop movement
        _anim.CrossFade(_idleAnimState, 0.1f); // Play Idle animation
        //_anim.CrossFade(_idleAnimState, 0.1f);
    }
    private void Hurt()
    {
        
    }
    private void Chase()
    {
        Debug.Log("Enemy is chasing");
        if (!target || _state == CharacterState.Hurt) return;

        
        _movement = movementDirectionSolver.GetDirectionToMove(steeringBehaviours, aiData);
        if (_movement == Vector2.zero)
        {
            _state = CharacterState.Idle;
            return;
        }
        // Handle rotation and movement...
        _rb.velocity = new Vector2(_movement.x * speedX, _movement.y * speedY);
        if (Vector2.Distance(transform.position, target.position) < minDistanceToAttack)
        {
            _state = CharacterState.WaitToAttack;
        }
        if (_movement.x < 0 && !Mathf.Approximately(transform.rotation.y, 180))
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (_movement.x > 0 && !Mathf.Approximately(transform.rotation.y, 0))
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        _anim.Play(_runAnimState);
        //_anim.CrossFade(_runAnimState, 0.1f);
    }
    

    
    public void TakeHit(float damageTaken)
    {
        if (_state == CharacterState.Attack) return;
        Debug.Log("Taking damage with: " + damageTaken + " damage");
        
        enemyHealth.TakeDamage(damageTaken);

        if (enemyHealth.currentHealth <= 0)
        {
            _currentHealthPoints = 0;
            // _rb.velocity = Vector2.zero;
            _state = CharacterState.Death;
            StartCoroutine(HitAndDie());
            
        }
        else
        {
            _anim.CrossFadeInFixedTime(_hitAnimState, 0f);
            _state = CharacterState.Hurt;
            StartCoroutine(RecoverFromHit());
        }
    }
    
    private IEnumerator HitAndDie()
    {
        CancelInvoke("UpdateEnemyStates");
        CancelInvoke("PerformDetection");
        _anim.CrossFade(_hitAnimState, 0.1f);
        float animCrossFaid = 0.4f;
        yield return new WaitForSeconds(_anim.GetCurrentAnimatorStateInfo(0).length - animCrossFaid);
        _anim.CrossFadeInFixedTime(_deathAnimState, animCrossFaid);
        _rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(_anim.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }

    private IEnumerator RecoverFromHit()
    {
        yield return new WaitForSeconds(0.2f);
        _state = CharacterState.Chase;
        _anim.Play(_runAnimState);
        //_anim.CrossFadeInFixedTime(_runAnimState, 0.2f);
    }


    

}
