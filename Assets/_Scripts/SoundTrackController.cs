using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrackController : MonoBehaviour
{
    public static SoundTrackController Instance { get; private set; }
    
    [SerializeField] AudioSource audioSource1;
    [SerializeField] AudioSource audioSource2;
    
    [SerializeField] private AudioClip mainSoundtrack;
    [SerializeField] private AudioClip bossSoundtrack;
    [SerializeField] private float fadeDuration = 1f;

    private bool _isFading;
    
    private BossDestructible bossDestructible;
    private bool isBossSoundtrackPlaying = false;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        bossDestructible = GameObject.FindGameObjectWithTag("BossDestructible").GetComponent<BossDestructible>();

        if (mainSoundtrack)
        {
            PlaySoundTrack(mainSoundtrack);
        }

    }

    private void Update()
    {
        if (bossDestructible.bossDestructibleActivated && !isBossSoundtrackPlaying && bossDestructible)
        {
            FadeTo(bossSoundtrack);
        }
        else if (!bossDestructible.bossDestructibleActivated && isBossSoundtrackPlaying && bossDestructible)
        {
            PlaySoundTrack(mainSoundtrack);
            isBossSoundtrackPlaying = false;
        }
    }

    private void PlaySoundTrack(AudioClip clip)
    {
        if (audioSource1.clip == clip) return;
        audioSource1.Stop();
        audioSource1.clip = clip;
        audioSource1.Play();
    }

    public void FadeTo(AudioClip newClip)
    {
        if (_isFading || !newClip) return;
        
        // Determine which audiosource is currently active or not
        AudioSource activeSource = audioSource1.isPlaying ? audioSource1 : audioSource2;
        AudioSource inactiveSource = activeSource == audioSource1 ? audioSource2 : audioSource1;
        
        // Set the new clip on the inactive source
        inactiveSource.clip = newClip;
        inactiveSource.Play();
        
        // Start Fading
        StartCoroutine(FadeBetweenSources(activeSource, inactiveSource));
    }

    private IEnumerator FadeBetweenSources(AudioSource fadeOutSource, AudioSource fadeInSource)
    {
        _isFading = true;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;
            
            fadeOutSource.volume = Mathf.Lerp(1f, 0f, progress);
            fadeInSource.volume = Mathf.Lerp(0f, 1f, progress);

            yield return null;
        }
        
        fadeOutSource.volume = 0f;
        fadeOutSource.Stop();
        fadeInSource.volume = 1f;

        _isFading = false;
    }
}
