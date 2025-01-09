using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip attackClip;

    public void PlayHurtClip()
    {
        audioSource.PlayOneShot(hurtClip);
    }

    public void PlayAttackClip()
    {
        audioSource.PlayOneShot(attackClip);
    }
}
