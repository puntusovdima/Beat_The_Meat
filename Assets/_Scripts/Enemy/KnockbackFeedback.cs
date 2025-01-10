using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class KnockbackFeedback : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private float knockbackForce = 10f, delay = 0.15f;
    
    // [SerializeField] private EnemyBeatController enemyBeatController;

    public UnityEvent onBegin, onDone;
    private EnemyBeatController _enemyBeatController;


    private void Start()
    {
        _enemyBeatController = GetComponent<EnemyBeatController>();
        
    }

    public void PlayFeedback(GameObject sender)
    {
        if (!gameObject.activeInHierarchy) return;
        // Debug.Log("Knockback feedback is activated");
        // if (GetComponent<EnemyBeatController>() is { } enemyBeatController)
        if (_enemyBeatController)
            _enemyBeatController.SetState(EnemyBeatController.CharacterState.Knockback);
        StopAllCoroutines();
        onBegin?.Invoke();
        Vector2 direction = (transform.position - sender.transform.position).normalized;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        StartCoroutine(Reset());
    }
    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(delay);
        rb.velocity = Vector2.zero;
        if (_enemyBeatController)
            _enemyBeatController.SetState(EnemyBeatController.CharacterState.Idle);
        onDone?.Invoke();
    }
}
