using UnityEngine;

public class DestructibleObject : MonoBehaviour, ITriggerEnter
{
    [SerializeField] private GameObject itemDrop;
    [SerializeField] private Transform dropSite;
    [SerializeField] Animator coinAnimator;
    private readonly int coinAnimHash = Animator.StringToHash("CoinAnimation");

    public void HitByPlayer(GameObject player)
    {
        Instantiate(itemDrop, dropSite.position, Quaternion.identity);
        coinAnimator.Play(coinAnimHash);
        Destroy(gameObject);
    }
}
