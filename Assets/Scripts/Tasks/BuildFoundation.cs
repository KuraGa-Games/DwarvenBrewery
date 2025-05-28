using UnityEngine;

public class BuildFoundation : MonoBehaviour
{
    [SerializeField] private string taskId = "build_foundation";
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Визуализация строительства")]
    [SerializeField] private GameObject completedVisual;
    [SerializeField] private GameObject inProgressVisual;

    [Header("Звук строительства")]
    [SerializeField] private AudioClip buildSound;
    [SerializeField] private float volume = 1f;

    private bool isPlayerInRange = false;
    private bool isBuilt = false;

    private void Update()
    {
        if (!isBuilt && isPlayerInRange && Input.GetKeyDown(interactKey))
        {
            TryBuild();
        }
    }

    private void TryBuild()
    {
        if (!TaskManager.Instance.IsTaskAvailable(taskId))
        {
            Debug.Log($"Задание '{taskId}' пока недоступно!");
            return;
        }

        TaskManager.Instance.ReportProgress(taskId);
        isBuilt = true;

        // Звук строительства
        if (buildSound != null)
        {
            AudioSource.PlayClipAtPoint(buildSound, transform.position, volume);
        }

        // Обновляем визуал
        if (inProgressVisual) inProgressVisual.SetActive(false);
        if (completedVisual) completedVisual.SetActive(true);
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
