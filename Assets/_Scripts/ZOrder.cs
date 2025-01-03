using UnityEngine;

public class ZOrder : MonoBehaviour
{
    [SerializeField] private Transform anchor;
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Canvas canvas;
    
    [SerializeField] private Collider2D terrainCollider;

    [SerializeField]
    private int layerOrder;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
       // terrainCollider = GameObject.Find("TerrainCollider").GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (!terrainCollider || terrainCollider.isActiveAndEnabled)
        {
            if (anchor)
            {
                layerOrder  = (int)(anchor.position.y * -10);
            }
            else
            {
                layerOrder = (int)(transform.position.y * -10);
            }

            if (spriteRenderer)
            {
                spriteRenderer.sortingOrder = layerOrder;
            }
            else if (canvas)
            {
                canvas.sortingOrder = layerOrder;
            }
        }
        else if (terrainCollider.isActiveAndEnabled == false) return;
    }
}
