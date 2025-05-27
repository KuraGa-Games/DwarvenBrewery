using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TaskUIManager : MonoBehaviour
{
    [SerializeField] private GameObject taskItemPrefab;
    [SerializeField] private Transform taskListContainer;

    private Dictionary<string, TaskUIItem> activeItems = new Dictionary<string, TaskUIItem>();

    private void Start()
    {
        RefreshTaskList();
    }

    private void Update()
    {
        RefreshTaskList();
    }

    void RefreshTaskList()
    {
        var availableTasks = TaskManager.Instance.GetAvailableTasks();

        // Удалим старые задачи, которых больше нет
        var currentIDs = availableTasks.Select(t => t.Data.taskID).ToList();
        var toRemove = activeItems.Keys.Where(id => !currentIDs.Contains(id)).ToList();
        foreach (var id in toRemove)
        {
            Destroy(activeItems[id].gameObject);
            activeItems.Remove(id);
        }

        foreach (var task in availableTasks)
        {
            string id = task.Data.taskID;

            if (!activeItems.ContainsKey(id))
            {
                var itemGO = Instantiate(taskItemPrefab, taskListContainer);
                var item = itemGO.GetComponent<TaskUIItem>();
                item.Setup(id, task.Data.taskName, task.IsCompleted);
                activeItems.Add(id, item);
            }
            else if (task.IsCompleted)
            {
                activeItems[id].MarkCompleted();
            }
        }
    }


    public void ClearTaskList()
    {
        foreach (var item in activeItems.Values)
        {
            Destroy(item.gameObject);
        }
        activeItems.Clear();
    }
}
