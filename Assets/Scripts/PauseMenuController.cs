using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // <<< ������ ��� ������ ��� ������ � UI ���������� ���� Slider

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject saveLoadPanel;

    [Header("Settings UI Elements")]
    [SerializeField] private Slider volumeSlider; // ������ �� ���� ������� ���������

    private bool isPaused = false;

    private void Start() // ���������� Start ��� Awake ��� ��������� ��������� ��������
    {
        // ���������� ��� ������, ����� ��������� UI ����, ������ ���� ���������
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (saveLoadPanel != null) saveLoadPanel.SetActive(false);

        // ��������� �������� ���������
        if (volumeSlider != null)
        {
            // ������������� ��������� �������� �������� �� ����������� ��������
            // ��� �� ������� ��������� MusicManager (���� �� ��������)
            if (MusicManager.Instance != null)
            {
                // ���������� GetCurrentTargetVolume �� MusicManager, ���� �� � ���� ����, 
                // ��� ������ ������� ��������� AudioSource
                // volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", MusicManager.Instance.GetCurrentTargetVolume()); // ���� ���� GetCurrentTargetVolume
                volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", MusicManager.Instance.GetComponent<AudioSource>().volume);
            }
            else
            {
                volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.4f); // ��������� ��������
            }

            // ��������� ��������� �� ��������� �������� ��������
            volumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        else
        {
            Debug.LogWarning("PauseMenuController: Volume Slider �� �������� � Inspector!");
        }
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // ���� ������� ������ �������� ��� ����������/��������, ������� ��������� ��
            if (settingsPanel.activeSelf || saveLoadPanel.activeSelf)
            {
                BackToPauseMenu(); // ��������� � �������� ���� �����
            }
            // ���� ������� �������� ���� �����, �� ������� �� ����
            else if (isPaused)
            {
                Resume();
            }
            // ���� ������ �� �������, ������ �� �����
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        // BackToPauseMenu(); // ��� �� ����� �����, �.�. Update ������������ ������� �� �������
        pauseMenuUI.SetActive(false);
        settingsPanel.SetActive(false); // ��������, ��� � ������ �������� �������
        saveLoadPanel.SetActive(false); // � ������ ����������/��������
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("Game Resumed");
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        // ������ �������� � ����������/�������� �� ��������� ������ ��� ������ �������� �����
        settingsPanel.SetActive(false);
        saveLoadPanel.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("Game Paused");
    }

    public void OpenSettings()
    {
        // pauseMenuUI.SetActive(false); // �������� ���� ����� ����� ���������� ������� ��� ����������
        settingsPanel.SetActive(true);
        saveLoadPanel.SetActive(false); // �������� ������ ������, ���� ��� ���� �������
        Debug.Log("Settings Opened");

        // ��������� �������� �������� ��� �������� ��������, ���� MusicManager ��� ��������������� �����
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

    public void BackToPauseMenu() // ���������� �������� "�����" �� Settings � Save/Load
    {
        saveLoadPanel.SetActive(false);
        settingsPanel.SetActive(false);
        // pauseMenuUI.SetActive(true); // ���������� �������� ���� �����, ���� ��� ���� ������
        Debug.Log("Back to Pause Menu");
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // ��������������� ����� ����� �������
        SceneManager.LoadScene("MainMenu"); // �������, ��� ��� ����� "MainMenu" ����������
        Debug.Log("Quit to Main Menu");
    }

    /// <summary>
    /// ����� ��� ��������� ��������� ������ ����� �������.
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(volume);
        }
        else
        {
            // ���� MusicManager �� ��������, ����� ������ ��������� ��������,
            // � MusicManager ��������� ��� ��� ��������� ������� ��� �������������.
            PlayerPrefs.SetFloat("MusicVolume", Mathf.Clamp(volume, 0f, 1f));
            Debug.LogWarning("PauseMenuController: MusicManager.Instance �� ������. ��������� ��������� � PlayerPrefs.");
        }
    }
}