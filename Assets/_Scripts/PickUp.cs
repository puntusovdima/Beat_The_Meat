using UnityEngine;

public class PickUp : MonoBehaviour, ITriggerEnter
{
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;

    protected GameObject _player;
    protected void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }

    public void HitByPlayer(GameObject player)
    {
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
        _player = player;
        ApplyPickUp();
    }

    protected virtual void ApplyPickUp()
    {
        FinishApplyPickUp();
    }

    protected virtual void FinishApplyPickUp()
    {
        Destroy(gameObject);
    }
}
