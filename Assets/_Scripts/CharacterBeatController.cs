using UnityEngine;

public class CharacterBeatController : MonoBehaviour
{
    [SerializeField] protected float speedX, speedY, jumpForce;

    [SerializeField] public int damage, maxHealthPoints;
    
    [SerializeField] protected Transform hitAnchor, bottomAnchor;
    [SerializeField] protected Vector2 hitSize;
    
    protected int _currentHealthPoints;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(hitAnchor.position, hitSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(bottomAnchor.position, new Vector2(0.1f, 0.1f));
    }

    public int GetDamage
    {
        get { return damage; }
    }
}
