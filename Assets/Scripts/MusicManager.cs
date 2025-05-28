using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip dayMusic;
    [SerializeField] private AudioClip nightMusic;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float maxVolume = 0.4f; // ��� ����� ������������ ��������� ��� ����-������

    private AudioSource audioSource;
    private bool isNight;
    private Coroutine fadeCoroutine;

    // ����������� ������ ��� ������� �� ������ �������� (��������, ��������)
    public static MusicManager Instance { get; private set; }

    private void Awake()
    {
        // ���������� Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) // ������� �������� � ��������, ���� ���
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.loop = true;
            audioSource.playOnAwake = false; // �� ��������� ��������
            audioSource.volume = 0f; // ������ � 0 ��� ������� �����
        }
        else
        {
            Destroy(gameObject); // ���������� ��������
            return;
        }
    }

    private void Start()
    {
        // ������������, ��� DayNightCycle ��� ���������� � �����
        // � �������� ������� ����� ����� �������� ������� ������������� ������������
        if (DayNightCycle.Instance != null)
        {
            isNight = DayNightCycle.Instance.IsNight;
            // ��������� ��������� ��������� �� ������ ������������ �������� ��� maxVolume
            audioSource.volume = PlayerPrefs.GetFloat("MusicVolume", maxVolume);
            PlayMusicWithoutFade(isNight ? nightMusic : dayMusic); // ������ ������ ��� �����, �� � ������������� ����������

            DayNightCycle.Instance.OnDayStarted += OnDayStarted;
            DayNightCycle.Instance.OnNightStarted += OnNightStarted;
        }
        else
        {
            Debug.LogError("MusicManager: DayNightCycle.Instance �� ������! ������ �� ����� �������������.");
            // ����� ��������� ��������� ������, ���� DayNightCycle �� �������� ��� ������
            // PlayMusicWithoutFade(dayMusic); 
        }
    }

    private void PlayMusicWithoutFade(AudioClip clipToPlay)
    {
        if (clipToPlay == null)
        {
            Debug.LogWarning("MusicManager: ������� ������������� null ���������.");
            return;
        }
        if (audioSource.clip == clipToPlay && audioSource.isPlaying) return; // ��� ������ ���� ����

        audioSource.Stop();
        audioSource.clip = clipToPlay;
        audioSource.Play();
        // ��������� ��� ������ ���� ����������� �� Start() ��� SetVolume()
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
        if (newClip == null)
        {
            Debug.LogWarning("MusicManager: ������� ������������� null ��������� � ������.");
            return;
        }
        if (audioSource.clip == newClip && audioSource.isPlaying && Mathf.Approximately(audioSource.volume, GetCurrentTargetVolume()))
        {
            // ��� ������ ���� ���� � ������ ����������
            return;
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeToNewClip(newClip));
    }

    private IEnumerator FadeToNewClip(AudioClip newClip)
    {
        float targetVolume = GetCurrentTargetVolume(); // ���������� ������� ��������� ���������

        // ������� ��������� �������� ����� (���� �� ������)
        if (audioSource.isPlaying)
        {
            float startingVolume = audioSource.volume;
            while (audioSource.volume > 0.01f) // �������� ����� �� ����
            {
                audioSource.volume -= Time.unscaledDeltaTime / fadeDuration * startingVolume; // ���������� unscaledDeltaTime ��� �����
                yield return null;
            }
        }

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        // ������� ���������� ��������� ������ �����
        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += Time.unscaledDeltaTime / fadeDuration * targetVolume;
            if (audioSource.volume > targetVolume) audioSource.volume = targetVolume; // ������������ ������
            yield return null;
        }
        audioSource.volume = targetVolume; // ������������� �����
    }

    /// <summary>
    /// ������������� ����� ��������� ������.
    /// </summary>
    /// <param name="volume">��������� �� 0.0 �� 1.0</param>
    public void SetVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp(volume, 0f, 1f); // ������������ ���������
        // maxVolume ������ ������������ ��� ������������ ������� ��� ����-����� ������,
        // � �������� ��������� audioSource ����� ����������� ���� �������
        audioSource.volume = clampedVolume;
        PlayerPrefs.SetFloat("MusicVolume", clampedVolume); // ��������� ���������
        Debug.Log("Music Volume set to: " + clampedVolume);
    }

    // ��������������� ����� ��� ��������� ������� ��������� (� ������ ����������� ���������)
    private float GetCurrentTargetVolume()
    {
        return PlayerPrefs.GetFloat("MusicVolume", maxVolume);
    }
}