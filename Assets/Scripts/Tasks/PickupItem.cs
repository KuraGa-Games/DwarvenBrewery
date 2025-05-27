using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private string taskId; // ID �������, ������� ����� ����������
    [SerializeField] private KeyCode pickupKey = KeyCode.E; // ������� ��� �������

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
        // ����������� �������� �� ������
        TaskManager.Instance.ReportProgress(taskId);

        // ����� �������� ����, ������ � �.�. ���

        // ������� ������
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
