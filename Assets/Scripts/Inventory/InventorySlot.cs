using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;  // В инспекторе перетащи сюда Image иконки предмета

    private InventoryItem currentItem;

    private void Start()
    {
        if (icon != null)
            icon.enabled = false;  // Скрываем иконку при старте (пустой слот)
    }

    public void AddItem(InventoryItem newItem)
    {
        currentItem = newItem;
        if (icon != null)
        {
            icon.sprite = newItem.icon;
            icon.enabled = true;
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }
    }

    public InventoryItem GetItem()
    {
        return currentItem;
    }
}
