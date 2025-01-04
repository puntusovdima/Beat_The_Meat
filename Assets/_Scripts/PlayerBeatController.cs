using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerBeatController : CharacterBeatController, ITriggerEnter
{
    private enum CharacterState
    {
        Idle,
        Walk,
        Jump,
        Attack,
        Fall,
        Hurt,
        Die
    }

    private readonly int _idleAnimState = Animator.StringToHash("Player_Idle");
    private readonly int _runAnimState = Animator.StringToHash("Player_Run");
    private readonly int _jumpAnimState = Animator.StringToHash("Player_Jump");
    private readonly int _attackAnimState = Animator.StringToHash("Player_Attack");
    private readonly int _fallAnimState = Animator.StringToHash("Player_Fall");
    private readonly int _hitAnimState = Animator.StringToHash("Player_Hit");
    private readonly int _deathAnimState = Animator.StringToHash("Player_Death");

    [SerializeField] protected LayerMask obstacleLayer, itemLayer;
    [SerializeField] Collider2D terrainCollider;
    private Rigidbody2D _rb;
    private Vector2 _movement;
    private float _floorLevel;
    private Animator _anim;

    private bool _canAttack;
    private bool _isDead;

    [SerializeField] Health _health;

    [SerializeField] private CharacterState _state;

    private void Awake()
    {
        _currentHealthPoints = maxHealthPoints;
        
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        //_health = GetComponent<Health>();
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
                _anim.CrossFadeInFixedTime(_idleAnimState, 0.2f);
            }
            else if (_movement != Vector2.zero && _state != CharacterState.Walk)
            {
                _state = CharacterState.Walk;
                _anim.CrossFadeInFixedTime(_runAnimState, 0.2f);
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
            terrainCollider.enabled = false;
            _state = CharacterState.Jump;
            _rb.gravityScale = 1;
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
            _floorLevel = transform.position.y - 0.0000001f;
            _anim.CrossFadeInFixedTime(_jumpAnimState, 0.2f);
        }
    }

    public void Attack()
    {

        if (_state == CharacterState.Walk || _state == CharacterState.Idle && _canAttack)
        {
            Collider2D[] results = Physics2D.OverlapBoxAll(hitAnchor.position, hitSize, 0);
            _canAttack = false;
            _state = CharacterState.Attack;
            _rb.velocity = Vector2.zero;
            _anim.CrossFadeInFixedTime(_attackAnimState, 0.2f);

            //var results = new Collider2D[] { };
            var size = Physics2D.OverlapBoxNonAlloc(hitAnchor.position, hitSize, 0, results);

            foreach (var result in results)
            {
                // layers are stored in an integer(4Bytes) 00000000 00000000 00001000 00000000
                if (((1 << result.gameObject.layer) & (hitLayerMask.value | obstacleLayer.value | itemLayer)) != 0)
                    result.GetComponentInParent<ITriggerEnter>()?.HitByPlayer(gameObject);
            }

            StartCoroutine(WaitForAttackAnimationToEnd(_anim.GetCurrentAnimatorStateInfo(0)));
        }
    }

    private IEnumerator WaitForAttackAnimationToEnd(AnimatorStateInfo stateInfo)
    {
        while (stateInfo.shortNameHash != _attackAnimState)
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
        _anim.CrossFadeInFixedTime(_idleAnimState, 0f);
        _canAttack = true;
    }

    public void TakeHit(float damageTaken)
    {
        if (_state == CharacterState.Attack) return;
        Debug.Log("Taking damage with: " + damageTaken + " damage(Player)");
        if (_state == CharacterState.Fall || _state == CharacterState.Jump)
        {
            Ground();
        }

        //_currentHealthPoints -= (int)damageTaken;
        _health.TakeDamage(damageTaken);

        if (_health.currentHealth <= 0)
        {
            _currentHealthPoints = 0;
            _state = CharacterState.Die;
            _isDead = true;
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
        _anim.CrossFadeInFixedTime(_hitAnimState, 0f);
        float animCrossFaid = 0.4f;
        yield return new WaitForSeconds(_anim.GetCurrentAnimatorStateInfo(0).length - animCrossFaid);
        _anim.CrossFadeInFixedTime(_deathAnimState, animCrossFaid);
    }

    private IEnumerator RecoverFromHit()
    {
        yield return new WaitForSeconds(0.2f);
        _state = CharacterState.Idle;
        _anim.CrossFadeInFixedTime(_idleAnimState, 0.2f);
    }

    private void Ground()
    {
        terrainCollider.enabled = true;
        _state = CharacterState.Idle;
        _rb.gravityScale = 0;
        _rb.velocity = new Vector2(_rb.velocity.x, 0);
        transform.position = new Vector3(transform.position.x, _floorLevel);
        _floorLevel = float.MinValue;
        _anim.CrossFadeInFixedTime(_idleAnimState, 0.2f);
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
                _anim.CrossFadeInFixedTime(_fallAnimState, 0.2f);
            }
        }
    }

    public void HitByPlayer(GameObject player)
    {
        Debug.Log("Player is hitting it's collider, wtf???(change to notImplemented after fixing");
        // throw new System.NotImplementedException();
    }

    public void HitByEnemy(GameObject enemy)
    {
        Debug.Log("Player is hit by " + enemy.gameObject.name);
        TakeHit(enemy.GetComponent<EnemyBeatController>().damage);
    }
}