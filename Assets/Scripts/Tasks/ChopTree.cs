using UnityEngine;

public class ChopTree : MonoBehaviour
{
    [SerializeField] private string taskId = "chop_tree";
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Спавн дров")]
    [SerializeField] private GameObject woodBundlePrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Звук рубки")]
    [SerializeField] private AudioClip chopSound;
    [SerializeField] private float volume = 1f;

    private bool isPlayerInRange = false;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D звук (если используется)
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
            Debug.Log($"Задание '{taskId}' пока недоступно!");
            return;
        }

        Chop();
    }

    private void Chop()
    {
        TaskManager.Instance.ReportProgress(taskId);

        // Звук
        if (chopSound != null)
        {
            AudioSource.PlayClipAtPoint(chopSound, transform.position, volume);
        }

        // Спавн дров
        if (woodBundlePrefab != null && spawnPoint != null)
        {
            Instantiate(woodBundlePrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Не задан woodBundlePrefab или spawnPoint в ChopTree!");
        }

        Destroy(gameObject); // Удаляем дерево
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
