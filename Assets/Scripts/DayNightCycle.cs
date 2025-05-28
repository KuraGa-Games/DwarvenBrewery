using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance { get; private set; }

    [SerializeField] private Light2D globalLight;

    [Header("Day Settings")]
    [SerializeField] private Color dayColor = new Color(1f, 0.95f, 0.8f); // тёплый
    [SerializeField] private float dayIntensity = 1f;

    [Header("Night Settings")]
    [SerializeField] private Color nightColor = new Color(0.247775f, 0.3464236f, 0.6037736f); // холодный
    [SerializeField] private float nightIntensity = 0.8f;


    [SerializeField] private GameObject urkaPrefab;
    [SerializeField] private Transform urkaSpawnPoint;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool IsNight { get; private set; }

    public void StartNight()
    {
        IsNight = true;
        Debug.Log("Наступила ночь");

        if (globalLight != null)
        {
            globalLight.color = nightColor;
            globalLight.intensity = nightIntensity;
        }

        SpawnUrka();
        ActivateNightTasks();
    }


    public void StartDay()
    {
        IsNight = false;
        Debug.Log("Наступил день");
        if (globalLight != null)
        {
            globalLight.color = dayColor;
            globalLight.intensity = dayIntensity;
        }

        // здесь можно перезапустить дневные задачи
    }


    private void SpawnUrka()
    {
        if (urkaPrefab != null && urkaSpawnPoint != null)
        {
            Instantiate(urkaPrefab, urkaSpawnPoint.position, Quaternion.identity);
            Debug.Log("Урка заспавнен");
        }
        else
        {
            Debug.LogWarning("Не назначен урка или точка спавна");
        }
    }

    private void ActivateNightTasks()
    {
        // Обновим доступные задачи (задания уже заранее загружены TaskManager'ом)
        TaskManager.Instance.ReportProgress("beat_orc", 0); // просто «пробуждаем» задание
    }

}
