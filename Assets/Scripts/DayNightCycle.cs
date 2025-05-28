using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance { get; private set; }

    [SerializeField] private Light2D globalLight;

    [Header("Day Settings")]
    [SerializeField] private Color dayColor = new Color(1f, 0.95f, 0.8f); // �����
    [SerializeField] private float dayIntensity = 1f;

    [Header("Night Settings")]
    [SerializeField] private Color nightColor = new Color(0.247775f, 0.3464236f, 0.6037736f); // ��������
    [SerializeField] private float nightIntensity = 0.8f;


    [SerializeField] private GameObject urkaPrefab;
    [SerializeField] private Transform urkaSpawnPoint;


    public event System.Action OnDayStarted;
    public event System.Action OnNightStarted;



    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool IsNight { get; private set; }

    public void StartNight()
    {
        IsNight = true;
        OnNightStarted?.Invoke();
        Debug.Log("��������� ����");

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
        OnDayStarted?.Invoke();
        Debug.Log("�������� ����");
        if (globalLight != null)
        {
            globalLight.color = dayColor;
            globalLight.intensity = dayIntensity;
        }

        // ����� ����� ������������� ������� ������
    }


    private void SpawnUrka()
    {
        if (urkaPrefab != null && urkaSpawnPoint != null)
        {
            Instantiate(urkaPrefab, urkaSpawnPoint.position, Quaternion.identity);
            Debug.Log("���� ���������");
        }
        else
        {
            Debug.LogWarning("�� �������� ���� ��� ����� ������");
        }
    }

    private void ActivateNightTasks()
    {
        // ������� ��������� ������ (������� ��� ������� ��������� TaskManager'��)
        TaskManager.Instance.ReportProgress("beat_orc", 0); // ������ ����������� �������
    }

}
