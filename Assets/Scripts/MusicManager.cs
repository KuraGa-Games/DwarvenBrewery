using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip dayMusic;
    [SerializeField] private AudioClip nightMusic;
    [SerializeField] private float fadeDuration = 2f; // сколько секунд длится затухание и нарастание
    [SerializeField] private float maxVolume = 0.4f;

    private AudioSource audioSource;
    private bool isNight;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;
    }

    private void Start()
    {
        isNight = DayNightCycle.Instance.IsNight;
        PlayMusicWithFade(isNight ? nightMusic : dayMusic);

        DayNightCycle.Instance.OnDayStarted += OnDayStarted;
        DayNightCycle.Instance.OnNightStarted += OnNightStarted;
    }

    private void OnDestroy()
    {
        if (DayNightCycle.Instance != null)
        {
            DayNightCycle.Instance.OnDayStarted -= OnDayStarted;
            DayNightCycle.Instance.OnNightStarted -= OnNightStarted;
        }
    }

    private void OnDayStarted()
    {
        isNight = false;
        PlayMusicWithFade(dayMusic);
    }

    private void OnNightStarted()
    {
        isNight = true;
        PlayMusicWithFade(nightMusic);
    }

    private void PlayMusicWithFade(AudioClip newClip)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeToNewClip(newClip));
    }

    private IEnumerator FadeToNewClip(AudioClip newClip)
    {
        // Плавное затухание
        while (audioSource.volume > 0f)
        {
            audioSource.volume -= Time.deltaTime / fadeDuration * maxVolume;
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        // Плавное увеличение громкости
        while (audioSource.volume < maxVolume)
        {
            audioSource.volume += Time.deltaTime / fadeDuration * maxVolume;
            yield return null;
        }

        audioSource.volume = maxVolume;
    }
}
