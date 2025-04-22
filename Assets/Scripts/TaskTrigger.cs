using UnityEngine;
using UnityEngine.InputSystem;

public class TaskTrigger : MonoBehaviour
{
    public string taskID;
    public bool requiresInput = true;

    private bool playerInside = false;

    private void Update()
    {
        if (playerInside && requiresInput && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryComplete();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            if (!requiresInput)
                TryComplete();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    private void TryComplete()
    {
        if (TaskManager.Instance.IsTaskActive(taskID))
        {
            TaskManager.Instance.TryCompleteTask(taskID);
            Destroy(gameObject); // Чтобы не сработало повторно
        }
    }
}
