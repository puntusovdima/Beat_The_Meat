using System.Collections;
using UnityEngine;

public class HealthPotion : PickUp
{
    [SerializeField] int healthRestored;
    [SerializeField] private float dissapearTime;
    public override void ApplyPickUp()
    {
        if (_player != null)
        {
            _player.GetComponent<Health>().Heal(healthRestored);
        }

        base.ApplyPickUp();
    }

    public void DestroyPotion()
    {
        // Potion goes gray to indicate it will dissapear
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.gray;
        StartCoroutine(Dissapear());
    }

    private IEnumerator Dissapear()
    {
        yield return new WaitForSeconds(dissapearTime);
        gameObject.SetActive(false);
    }
}
