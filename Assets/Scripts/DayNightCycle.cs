using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance { get; private set; }

    [SerializeField] private Light2D globalLight;

    [Header("Day Settings")]
    [SerializeField] private Color dayColor = new Color(1f, 0.95f, 0.8f); // тЄплый
    [SerializeField] private float dayIntensity = 1f;

    [Header("Night Settings")]
    [SerializeField] private Color nightColor = new Color(0.247775f, 0.3464236f, 0.6037736f); // холодный
    [SerializeField] private float nightIntensity = 0.8f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool IsNight { get; private set; }

    public void StartNight()
    {
        IsNight = true;
        Debug.Log("Ќаступила ночь");
        if (globalLight != null)
        {
            globalLight.color = nightColor;
            globalLight.intensity = nightIntensity;
        }

        // здесь можно запускать ночные задани€
    }

    public void StartDay()
    {
        IsNight = false;
        Debug.Log("Ќаступил день");
        if (globalLight != null)
        {
            globalLight.color = dayColor;
            globalLight.intensity = dayIntensity;
        }

        // здесь можно перезапустить дневные задачи
    }
}
