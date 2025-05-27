using UnityEngine;

public class BuildFoundation : MonoBehaviour
{
    [SerializeField] private string taskId = "build_foundation";
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private GameObject completedVisual; // Готовый фундамент
    [SerializeField] private GameObject inProgressVisual; // Недостроенное/пустое место

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

        // Переключаем визуал
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
