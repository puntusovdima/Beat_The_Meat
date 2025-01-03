using UnityEngine;

public interface ITriggerEnter
{
    public void HitByPlayer(GameObject player);
    public void HitByEnemy(GameObject enemy);
}
