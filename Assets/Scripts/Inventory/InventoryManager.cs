using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public InventorySlot[] slots;    // В инспекторе назначь 5 слотов
    public Text messageText;         // Текст для сообщений (например, "Инвентарь заполнен!")

    private int selectedSlotIndex = -1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Очистим все слоты при старте
        foreach (var slot in slots)
            slot.ClearSlot();

        // Очистим сообщение
        if (messageText != null)
            messageText.text = "";
    }

    public bool AddItem(InventoryItem item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetItem() == null)
            {
                slots[i].AddItem(item);
                return true;
            }
        }

        ShowMessage("Инвентарь заполнен!");
        return false;
    }

    public void UseItem(int index)
    {
        if (index < 0 || index >= slots.Length)
            return;

        var item = slots[index].GetItem();
        if (item != null)
        {
            Debug.Log("Использован предмет: " + item.itemName);
            selectedSlotIndex = index;
        }
    }

    public void DropSelectedItem()
    {
        if (selectedSlotIndex != -1)
        {
            slots[selectedSlotIndex].ClearSlot();
            selectedSlotIndex = -1;
        }
    }

    private void ShowMessage(string msg)
    {
        if (messageText != null)
        {
            messageText.text = msg;
            CancelInvoke(nameof(ClearMessage));
            Invoke(nameof(ClearMessage), 2f);
        }
    }

    private void ClearMessage()
    {
        if (messageText != null)
            messageText.text = "";
    }
}
