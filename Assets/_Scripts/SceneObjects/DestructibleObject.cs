using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DestructibleObject : MonoBehaviour, ITriggerEnter
{
    [SerializeField] private GameObject itemDrop;
    [SerializeField] private Transform dropSite;
    [SerializeField] Animator coinAnimator;
    private readonly int coinAnimHash = Animator.StringToHash("CoinAnimation");
    [SerializeField] private float dropDelay = 0.5f;
    
    public UnityEvent onHit;

    public void HitByPlayer(GameObject player)
    {
        onHit.Invoke();
        StartCoroutine(GetTheBonus());
    }

    public void HitByEnemy(GameObject enemy)
    {
        throw new NotImplementedException();
    }

    private IEnumerator GetTheBonus()
    {
        yield return new WaitForSeconds(dropDelay);
        Instantiate(itemDrop, dropSite.position, Quaternion.identity);
        coinAnimator.enabled = true;
        coinAnimator.Play(coinAnimHash);
        Destroy(gameObject);
    }
}
