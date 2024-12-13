using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D), typeof(PlayerBeatInput))]
public class PlayerBeatController : CharacterBeatController
{
    private enum CharacterState {Idle, Walk, Jump, Attack, Fall, Hurt, Die}
    
    private readonly int IdleAnimState = Animator.StringToHash("Player_Idle");
    private readonly int RunAnimState = Animator.StringToHash("Player_Run");
    private readonly int JumpAnimState = Animator.StringToHash("Player_Jump");
    private readonly int AttackAnimState = Animator.StringToHash("Player_Attack");
    private readonly int FallAnimState = Animator.StringToHash("Player_Fall");
    private readonly int HitAnimState = Animator.StringToHash("Player_Hit");
    private readonly int DeathAnimState = Animator.StringToHash("Player_Death");


    
    
    private Rigidbody2D _rb;
    private Vector2 _movement;
    private float _floorLevel;
    private Animator _anim;

    private bool _canAttack;
    
    [SerializeField] private CharacterState _state;

    private void Awake()
    {
        _currentHealthPoints = maxHealthPoints;
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        _rb.gravityScale = 0;

        _floorLevel = float.MinValue;
        _state = CharacterState.Idle;
        _canAttack = true;
    }

    public void MoveAction(Vector2 movement)
    {
        _movement = movement;
        _movement.x *= speedX;
        _movement.y *= speedY;

        if (_state != CharacterState.Attack)
        {
            if (_movement.x < 0 && !Mathf.Approximately(transform.rotation.y, 180))
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (_movement.x > 0 && !Mathf.Approximately(transform.rotation.y, 0))
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        
        if (_state == CharacterState.Idle || _state == CharacterState.Walk)
        {
            if (_movement == Vector2.zero && _state != CharacterState.Idle)
            {
                _state = CharacterState.Idle;
                _anim.CrossFadeInFixedTime(IdleAnimState, 0.2f);
            }
            else if (_movement != Vector2.zero && _state != CharacterState.Walk)
            {
                _state = CharacterState.Walk;
                _anim.CrossFadeInFixedTime(RunAnimState, 0.2f);
            }
            _rb.velocity = _movement;
        }
        else
        {
            _rb.velocity = new Vector2(_movement.x, _rb.velocity.y);
        }
    }

    public void Jump()
    {
        if (_state == CharacterState.Walk || _state == CharacterState.Idle)
        {
            _state = CharacterState.Jump;
            _rb.gravityScale = 1;
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
            _floorLevel = transform.position.y - 0.0000001f;
            _anim.CrossFadeInFixedTime(JumpAnimState, 0.2f);
        }
    }

    public void Attack()
    {
        Collider2D[] results = Physics2D.OverlapBoxAll(hitAnchor.position, hitSize, 0);
        
        if (_state == CharacterState.Walk || _state == CharacterState.Idle && _canAttack)
        {
            _canAttack = false;
            _state = CharacterState.Attack;
            _rb.velocity = Vector2.zero;
            _anim.CrossFadeInFixedTime(AttackAnimState, 0.2f);

            //var results = new Collider2D[] { };
            var size = Physics2D.OverlapBoxNonAlloc(hitAnchor.position, hitSize, 0, results);

            for (int i = 0; i < results.Length; i++)
            {
                results[i].GetComponent<ITriggerEnter>()?.HitByPlayer(gameObject);
            }

            StartCoroutine(WaitForAttackAnimationToEnd(_anim.GetCurrentAnimatorStateInfo(0)));
        }
    }

    private IEnumerator WaitForAttackAnimationToEnd(AnimatorStateInfo stateInfo)
    {
        while (stateInfo.shortNameHash != AttackAnimState)
        {
            yield return null;
            stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        }
        while (stateInfo.normalizedTime < 1f)
        {
            yield return null;
            stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        }
        _state = CharacterState.Idle;
        _anim.CrossFadeInFixedTime(IdleAnimState, 0f);
        _canAttack = true;
    }

    public void TakeDamage(float damageTaken)
    {
        if (_state == CharacterState.Attack) return;
        Debug.Log("Taking damage with: " + damageTaken + " damage");
        if (_state == CharacterState.Fall || _state == CharacterState.Jump)
        {
            Ground();
        }
        
        _currentHealthPoints -= (int)damageTaken;
        _anim.CrossFadeInFixedTime(HitAnimState, 0f);

        if (_currentHealthPoints <= 0)
        {
            _currentHealthPoints = 0;
            _state = CharacterState.Die;
            _anim.CrossFadeInFixedTime(DeathAnimState, 0f);
        }
        else
        {
            _anim.CrossFadeInFixedTime(HitAnimState, 0f);
            _state = CharacterState.Hurt;
            _rb.velocity = Vector2.zero;
        }
        
    }

    private void Ground()
    {
        _state = CharacterState.Idle;
        _rb.gravityScale = 0;
        _rb.velocity = new Vector2(_rb.velocity.x, 0);
        transform.position = new Vector3(transform.position.x, _floorLevel);
        _floorLevel = float.MinValue;
        _anim.CrossFadeInFixedTime(IdleAnimState, 0.2f);
    }

    private void Update()
    {
        if (_state == CharacterState.Fall)
        {
            if (transform.position.y < _floorLevel)
            {
                Ground();
            }
        }
        else if (_state == CharacterState.Jump)
        {
            if (_rb.velocity.y < 0)
            {
                _state = CharacterState.Fall;
                _anim.CrossFadeInFixedTime(FallAnimState, 0.2f);
            }
        }
    }
}
