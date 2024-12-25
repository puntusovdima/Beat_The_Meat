
using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private float scoreOnTakedown;
    
    protected override void Death()
    {
        //Muerte de un enemigo
        Debug.Log("Enemy down");
        GameManager.Instance.AddScore(scoreOnTakedown);
    }
}
