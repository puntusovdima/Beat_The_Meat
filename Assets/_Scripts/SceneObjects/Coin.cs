using UnityEngine;
using UnityEngine.Events;

public class Coin : MonoBehaviour
{
    public UnityEvent coinCollected;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            coinCollected?.Invoke();
            Destroy(gameObject);
        }
    }

    public void TestMethod()
    {
        Debug.Log("Coin collected!");
    }
}
