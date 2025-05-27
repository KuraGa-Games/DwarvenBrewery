using UnityEngine;

public class ChopTree : MonoBehaviour
{
    [SerializeField] private string taskId = "chop_tree";
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Спавн дров")]
    [SerializeField] private GameObject woodBundlePrefab;
    [SerializeField] private Transform spawnPoint;

    private bool isPlayerInRange = false;

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

        // Спавним охапку дров
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
