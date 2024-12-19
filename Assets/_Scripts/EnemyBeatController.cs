using UnityEngine;


public class EnemyBeatController : MonoBehaviour, ITriggerEnter 
{
    private enum CharacterState { Chase, Attack, Hurt, WaitToAttack, Death}

    [SerializeField] private float minTimeBeforeAttack, minDistanceToAttack, maxTimeBeforeAttack;
    
    private CharacterState _state;

    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Animator _anim;
    private Transform _target;
    
    public void HitByPlayer(GameObject player)
    {
        throw new System.NotImplementedException();
    }
}
