using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;   // Название предмета
    public Sprite icon;       // Иконка для UI

    public InventoryItem(string name, Sprite icon)
    {
        this.itemName = name;
        this.icon = icon;
    }
}
