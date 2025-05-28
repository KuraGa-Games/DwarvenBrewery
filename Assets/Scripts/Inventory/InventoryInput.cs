using UnityEngine;

public class InventoryInput : MonoBehaviour
{
    private void Update()
    {
        // Клавиши 1-5 для выбора предмета в инвентаре
        for (int i = 0; i < 5; i++)
        {
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
            {
                InventoryManager.Instance.UseItem(i);
            }
        }

        // Клавиша G для выброса выбранного предмета
        if (Input.GetKeyDown(KeyCode.G))
        {
            InventoryManager.Instance.DropSelectedItem();
        }
    }
}
