using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private string taskId;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;

    [Header("Звук подбора")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private float volume = 1f;

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
        // Звук
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
        }

        // Продвигаем задачу
        TaskManager.Instance.ReportProgress(taskId);

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
