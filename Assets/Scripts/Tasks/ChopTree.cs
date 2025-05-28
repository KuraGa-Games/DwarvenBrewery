using UnityEngine;

public class ChopTree : MonoBehaviour
{
    [SerializeField] private string taskId = "chop_tree";
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("����� ����")]
    [SerializeField] private GameObject woodBundlePrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("���� �����")]
    [SerializeField] private AudioClip chopSound;
    [SerializeField] private float volume = 1f;

    private bool isPlayerInRange = false;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D ���� (���� ������������)
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(interactKey))
        {
            TryChop();
        }
    }

    private void TryChop()
    {
        if (!TaskManager.Instance.IsTaskAvailable(taskId))
        {
            Debug.Log($"������� '{taskId}' ���� ����������!");
            return;
        }

        Chop();
    }

    private void Chop()
    {
        TaskManager.Instance.ReportProgress(taskId);

        // ����
        if (chopSound != null)
        {
            AudioSource.PlayClipAtPoint(chopSound, transform.position, volume);
        }

        // ����� ����
        if (woodBundlePrefab != null && spawnPoint != null)
        {
            Instantiate(woodBundlePrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("�� ����� woodBundlePrefab ��� spawnPoint � ChopTree!");
        }

        Destroy(gameObject); // ������� ������
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
