using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // <<< ДОБАВЬ ЭТУ СТРОКУ для работы с UI элементами типа Slider

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject saveLoadPanel;

    [Header("Settings UI Elements")]
    [SerializeField] private Slider volumeSlider; // Ссылка на твой слайдер громкости

    private bool isPaused = false;

    private void Start() // Используем Start или Awake для начальной настройки слайдера
    {
        // Изначально все панели, кроме основного UI игры, должны быть выключены
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (saveLoadPanel != null) saveLoadPanel.SetActive(false);

        // Настройка слайдера громкости
        if (volumeSlider != null)
        {
            // Устанавливаем начальное значение слайдера из сохраненных настроек
            // или из текущей громкости MusicManager (если он доступен)
            if (MusicManager.Instance != null)
            {
                // Используем GetCurrentTargetVolume из MusicManager, если он у тебя есть, 
                // или просто текущую громкость AudioSource
                // volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", MusicManager.Instance.GetCurrentTargetVolume()); // Если есть GetCurrentTargetVolume
                volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", MusicManager.Instance.GetComponent<AudioSource>().volume);
            }
            else
            {
                volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.4f); // Дефолтное значение
            }

            // Добавляем слушателя на изменение значения слайдера
            volumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        else
        {
            Debug.LogWarning("PauseMenuController: Volume Slider не назначен в Inspector!");
        }
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Если открыта панель настроек или сохранения/загрузки, сначала закрываем их
            if (settingsPanel.activeSelf || saveLoadPanel.activeSelf)
            {
                BackToPauseMenu(); // Вернуться в основное меню паузы
            }
            // Если открыто основное меню паузы, то выходим из него
            else if (isPaused)
            {
                Resume();
            }
            // Если ничего не открыто, ставим на паузу
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        // BackToPauseMenu(); // Уже не нужно здесь, т.к. Update обрабатывает возврат из подменю
        pauseMenuUI.SetActive(false);
        settingsPanel.SetActive(false); // Убедимся, что и панель настроек закрыта
        saveLoadPanel.SetActive(false); // И панель сохранения/загрузки
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("Game Resumed");
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        // Панели настроек и сохранения/загрузки по умолчанию скрыты при первом открытии паузы
        settingsPanel.SetActive(false);
        saveLoadPanel.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("Game Paused");
    }

    public void OpenSettings()
    {
        // pauseMenuUI.SetActive(false); // Основное меню паузы может оставаться видимым или скрываться
        settingsPanel.SetActive(true);
        saveLoadPanel.SetActive(false); // Скрываем другую панель, если она была открыта
        Debug.Log("Settings Opened");

        // Обновляем значение слайдера при открытии настроек, если MusicManager был инициализирован позже
        if (volumeSlider != null && MusicManager.Instance != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", MusicManager.Instance.GetComponent<AudioSource>().volume);
        }
        else if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.4f);
        }
    }

    public void OpenSaveLoadMenu()
    {
        // pauseMenuUI.SetActive(false);
        saveLoadPanel.SetActive(true);
        settingsPanel.SetActive(false);
        Debug.Log("Save/Load Menu Opened");
    }

    public void BackToPauseMenu() // Вызывается кнопками "Назад" из Settings и Save/Load
    {
        saveLoadPanel.SetActive(false);
        settingsPanel.SetActive(false);
        // pauseMenuUI.SetActive(true); // Показываем основное меню паузы, если оно было скрыто
        Debug.Log("Back to Pause Menu");
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Восстанавливаем время перед выходом
        SceneManager.LoadScene("MainMenu"); // Убедись, что имя сцены "MainMenu" правильное
        Debug.Log("Quit to Main Menu");
    }

    /// <summary>
    /// Метод для изменения громкости музыки через слайдер.
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(volume);
        }
        else
        {
            // Если MusicManager не доступен, можно просто сохранить значение,
            // а MusicManager подхватит его при следующем запуске или инициализации.
            PlayerPrefs.SetFloat("MusicVolume", Mathf.Clamp(volume, 0f, 1f));
            Debug.LogWarning("PauseMenuController: MusicManager.Instance не найден. Громкость сохранена в PlayerPrefs.");
        }
    }
}