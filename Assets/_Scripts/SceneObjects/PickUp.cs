using UnityEngine;
using UnityEngine.Events;

public class PickUp : MonoBehaviour, ITriggerEnter
{
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;

    protected GameObject _player;
    public UnityEvent itemCollected;

    protected void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _player = collision.gameObject;
            itemCollected?.Invoke();
        }
    }

    public void HitByPlayer(GameObject player)
    {
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
        _player = player;
        // ApplyPickUp();
        itemCollected?.Invoke();
    }

    public void HitByEnemy(GameObject enemy)
    {
        Destroy(gameObject);
    }

    public virtual void ApplyPickUp()
    {
        FinishApplyPickUp();
    }

    protected virtual void FinishApplyPickUp()
    {
        Destroy(gameObject);
    }
}
