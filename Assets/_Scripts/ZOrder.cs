using UnityEngine;

public class ZOrder : MonoBehaviour
{
    [SerializeField] private Transform anchor;
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [SerializeField] private Collider2D terrainCollider;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
       // terrainCollider = GameObject.Find("TerrainCollider").GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (terrainCollider.isActiveAndEnabled == false) return;
        if (anchor)
        {
            spriteRenderer.sortingOrder = (int)(anchor.position.y * -10);
        }
        else
        {
            spriteRenderer.sortingOrder = (int)(transform.position.y * -10);
        }
        
    }
}
