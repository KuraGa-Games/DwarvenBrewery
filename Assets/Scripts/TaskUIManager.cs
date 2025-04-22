/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // если используешь TextMeshPro

public class TaskUIManager : MonoBehaviour
{
    [SerializeField] private GameObject taskItemPrefab;
    [SerializeField] private Transform taskListContent;

    private Dictionary<string, GameObject> taskItems = new Dictionary<string, GameObject>();

    public void Initialize(List<TaskData> tasks)
    {
        foreach (var task in tasks)
        {
            GameObject item = Instantiate(taskItemPrefab, taskListContent);
            UpdateTaskItemUI(item, task);
            taskItems[task.taskID] = item;
        }
    }

    public void UpdateTask(TaskData task)
    {
        if (taskItems.TryGetValue(task.taskID, out GameObject item))
        {
            UpdateTaskItemUI(item, task);
        }
    }

    private void UpdateTaskItemUI(GameObject item, TaskData task)
    {
        TextMeshProUGUI text = item.GetComponentInChildren<TextMeshProUGUI>();
        if (task.isCompleted)
        {
            text.text = $"<s>{task.description}</s>";
            text.color = Color.gray;
        }
        else
        {
            text.text = task.description;
            text.color = Color.white;
        }
    }
}
*/