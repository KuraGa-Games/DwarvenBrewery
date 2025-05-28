using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip dayMusic;
    [SerializeField] private AudioClip nightMusic;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float maxVolume = 0.4f; // Это будет максимальная громкость при авто-фейдах

    private AudioSource audioSource;
    private bool isNight;
    private Coroutine fadeCoroutine;

    // Статическая ссылка для доступа из других скриптов (например, настроек)
    public static MusicManager Instance { get; private set; }

    private void Awake()
    {
        // Реализация Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) // Добавим проверку и создание, если нет
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.loop = true;
            audioSource.playOnAwake = false; // Мы управляем запуском
            audioSource.volume = 0f; // Начнем с 0 для первого фейда
        }
        else
        {
            Destroy(gameObject); // Уничтожаем дубликат
            return;
        }
    }

    private void Start()
    {
        // Предполагаем, что DayNightCycle уже существует в сцене
        // В реальном проекте нужна более надежная система инициализации зависимостей
        if (DayNightCycle.Instance != null)
        {
            isNight = DayNightCycle.Instance.IsNight;
            // Установим начальную громкость на основе сохраненного значения или maxVolume
            audioSource.volume = PlayerPrefs.GetFloat("MusicVolume", maxVolume);
            PlayMusicWithoutFade(isNight ? nightMusic : dayMusic); // Первый запуск без фейда, но с установленной громкостью

            DayNightCycle.Instance.OnDayStarted += OnDayStarted;
            DayNightCycle.Instance.OnNightStarted += OnNightStarted;
        }
        else
        {
            Debug.LogError("MusicManager: DayNightCycle.Instance не найден! Музыка не будет переключаться.");
            // Можно запустить дефолтную музыку, если DayNightCycle не критичен для старта
            // PlayMusicWithoutFade(dayMusic); 
        }
    }

    private void PlayMusicWithoutFade(AudioClip clipToPlay)
    {
        if (clipToPlay == null)
        {
            Debug.LogWarning("MusicManager: Попытка воспроизвести null аудиоклип.");
            return;
        }
        if (audioSource.clip == clipToPlay && audioSource.isPlaying) return; // Уже играет этот трек

        audioSource.Stop();
        audioSource.clip = clipToPlay;
        audioSource.Play();
        // Громкость уже должна быть установлена из Start() или SetVolume()
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
            Debug.LogWarning("MusicManager: Попытка воспроизвести null аудиоклип с фейдом.");
            return;
        }
        if (audioSource.clip == newClip && audioSource.isPlaying && Mathf.Approximately(audioSource.volume, GetCurrentTargetVolume()))
        {
            // Уже играет этот трек с нужной громкостью
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
        float targetVolume = GetCurrentTargetVolume(); // Используем текущую настройку громкости

        // Плавное затухание текущего трека (если он играет)
        if (audioSource.isPlaying)
        {
            float startingVolume = audioSource.volume;
            while (audioSource.volume > 0.01f) // Затухаем почти до нуля
            {
                audioSource.volume -= Time.unscaledDeltaTime / fadeDuration * startingVolume; // Используем unscaledDeltaTime для паузы
                yield return null;
            }
        }

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        // Плавное увеличение громкости нового трека
        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += Time.unscaledDeltaTime / fadeDuration * targetVolume;
            if (audioSource.volume > targetVolume) audioSource.volume = targetVolume; // Ограничиваем сверху
            yield return null;
        }
        audioSource.volume = targetVolume; // Устанавливаем точно
    }

    /// <summary>
    /// Устанавливает общую громкость музыки.
    /// </summary>
    /// <param name="volume">Громкость от 0.0 до 1.0</param>
    public void SetVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp(volume, 0f, 1f); // Ограничиваем громкость
        // maxVolume теперь используется как максимальный уровень при авто-смене треков,
        // а реальная громкость audioSource будет управляться этим методом
        audioSource.volume = clampedVolume;
        PlayerPrefs.SetFloat("MusicVolume", clampedVolume); // Сохраняем настройку
        Debug.Log("Music Volume set to: " + clampedVolume);
    }

    // Вспомогательный метод для получения целевой громкости (с учетом сохраненной настройки)
    private float GetCurrentTargetVolume()
    {
        return PlayerPrefs.GetFloat("MusicVolume", maxVolume);
    }
}