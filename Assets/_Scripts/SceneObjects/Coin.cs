using UnityEngine;
using UnityEngine.Events;

public class Coin : PickUp
{

    public void TestMethod()
    {
        Debug.Log("Coin collected!");
        Destroy(this.gameObject);
    }
}
