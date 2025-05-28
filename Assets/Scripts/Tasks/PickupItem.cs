using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private string taskId;            // ID задачи (можно пустое, если не нужно)
    [SerializeField] private KeyCode pickupKey = KeyCode.E; // Клавиша для подбора предмета
    [SerializeField] private string itemName;          // Название предмета для инвентаря
    [SerializeField] private Sprite itemIcon;          // Иконка предмета для инвентаря

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
        // Если у тебя есть TaskManager - сообщаем ему, иначе можно удалить эту строку
        if (!string.IsNullOrEmpty(taskId))
            TaskManager.Instance.ReportProgress(taskId);

        // Создаем новый предмет для инвентаря
        InventoryItem newItem = new InventoryItem(itemName, itemIcon);

        // Пытаемся добавить в инвентарь
        if (InventoryManager.Instance.AddItem(newItem))
        {
            // Успешно добавили — удаляем объект с карты
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Инвентарь заполнен!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = false;
    }
}
