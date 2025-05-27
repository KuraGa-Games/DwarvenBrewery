using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private string taskId; // ID задания, которое будет продвинуто
    [SerializeField] private KeyCode pickupKey = KeyCode.E; // Клавиша для подбора

    private bool isPlayerInRange = false;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(pickupKey))
        {
            Pickup();
        }
    }

    private void Pickup()
    {
        // Засчитываем прогресс по задаче
        TaskManager.Instance.ReportProgress(taskId);

        // Можно добавить звук, эффект и т.д. тут

        // Удаляем объект
        Destroy(gameObject);
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
