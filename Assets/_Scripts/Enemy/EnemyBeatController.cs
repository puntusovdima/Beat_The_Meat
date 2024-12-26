using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;


public class EnemyBeatController : CharacterBeatController, ITriggerEnter 
{
    private enum CharacterState { Chase, Attack, Hurt, WaitToAttack, Death, Idle }

    // later attack delay should be randomly chosen(or clamped) between minTimeBeforeAttack and maxTimeBeforeAttack
    [SerializeField] private float minTimeBeforeAttack, minDistanceToAttack, maxTimeBeforeAttack;
    
    // AI detection stuff
    [SerializeField]
    private float detectionDelay = 0.01f, aiUpdateDelay = 0.06f, attackDelay = 1f;
    [SerializeField] private AIData _aiData;
    [SerializeField] private List<SteeringBehaviour> _steeringBehaviours;
    [SerializeField] List<Detector> _detectors;
    private float attackDistance = 0.5f;

    [SerializeField] private Vector2 movementInput;
    
    [SerializeField] ContextSolver _movementDirectionSolver;

    
    private CharacterState _state;
    
    private readonly int _idleAnimState = Animator.StringToHash("Enemy_Idle");
    private readonly int _runAnimState = Animator.StringToHash("Enemy_Run");
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
    private float _enemyUpdateRate = 0.05f;
    
    bool lostTarget = false;

    
   
    
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
    
    private void Start()
    {
        InvokeRepeating("PerformDetection", 0, detectionDelay);
        InvokeRepeating("UpdateEnemyStates", 0, aiUpdateDelay);
     }
    private void PerformDetection()
    {
        foreach (Detector detector in _detectors)
        {
            detector.Detect(_aiData);
        }
    }
    
    private void UpdateEnemyStates()
    {
        //_target.position = _player.position;
        switch (_state)
        {
            case CharacterState.Idle:
                Idle();
                break;
            case CharacterState.Chase:
                Chase();
                break;
            case CharacterState.Attack:
                //Attack();
                break;
            case CharacterState.Hurt:
                //Hurt();
                break;
            case CharacterState.WaitToAttack:
               // WaitToAttack();
                break;
            case CharacterState.Death:
               // Death();
                break;
        }
    }

    private void Idle()
    {
        if (_aiData.currentTarget || _aiData.GetTargetsCount() > 0)
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
        if (!_target || _state == CharacterState.Hurt) return;

        
        _movement = _movementDirectionSolver.GetDirectionToMove(_steeringBehaviours, _aiData);
        if (_movement == Vector2.zero)
        {
            _state = CharacterState.Idle;
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
        _anim.Play(_runAnimState);
        //_anim.CrossFade(_runAnimState, 0.1f);
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
